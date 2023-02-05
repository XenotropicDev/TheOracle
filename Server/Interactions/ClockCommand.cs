using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Server.Data;
using Server.DiscordServer;
using Server.Interactions.Helpers;
using TheOracle2.GameObjects;

namespace TheOracle2;

// same as ProgressTrackCommandGroup, but as a single command with progress type set via a parameter. only one should be enabled at a time.

// [DontAutoRegister]
public class ClockCommand : InteractionModuleBase
{
    public IEmoteRepository Emotes { get; set; }
    public IMoveRepository Moves { get; set; }
    public PlayerDataFactory DataFactory { get; set; }
    public ApplicationContext DbContext { get; set; }

    public ClockCommand(ApplicationContext dbContext)
    {
        DbContext = dbContext;
    }

    [SlashCommand("clock", "Set a campaign clock, tension clock, or scene challenge (p. 230).")]
    public async Task BuildClock(
        [Summary(description: "The type of clock: campaign clock (p. 231), tension clock (p. 234), or scene challenge (p. 235).")]
        [Choice("Campaign clock","campaign-clock"),
        Choice("Tension clock", "tension-clock"),
        //Choice("Scene challenge", "scene-challenge")
        ]
        string clockType,
        [Summary(description: "A title that makes it clear what project is complete or event triggered when the clock is filled.")]
        string title,
        [Summary(description: "The number of clock segments.")]
        ClockSize segments,
        [Summary(description: "An optional description.")]
        string description=""
    )
    {
        switch (clockType)
        {
            case "campaign-clock":
                await BuildCampaignClock(title, segments, description);
                break;

            case "tension-clock":
                await BuildTensionClock(title, segments, description);
                break;

            case "scene-challenge":
                await BuildSceneChallenge(title, segments, description);
                break;
        }
    }

    // [SlashCommand("campaign", "Set a campaign clock to resolve objectives and actions in the background of your campaign (p. 231).")]
    public async Task BuildCampaignClock(
      [Summary(description: "A title that makes it clear what project is complete or event triggered when the clock is filled.")]
    string title,
      [Summary(description: "The number of clock segments.")]
    ClockSize segments,
      [Summary(description: "An optional description.")]
    string description=""
    )
    {
        CampaignClock campaignClock = new(segments, 0, title, description);
        await campaignClock.EntityAsResponse(RespondAsync);
    }

    // [SlashCommand("tension", "Set a tension clock: a smaller-scope clock to fill as you suffer setbacks or fail to act (p. 234).")]
    public async Task BuildTensionClock(
      [Summary(description: "A title for the tension clock.")]
    string title,
      [Summary(description: "The number of clock segments. Imminent danger or deadline: 4-6. Longer term threat: 8-10.")]
    ClockSize segments,
      [Summary(description: "An optional description.")]
    string description=""
    )
    {
        TensionClock tensionClock = new(segments, 0, title, description);
        await tensionClock.EntityAsResponse(RespondAsync);
    }

    // [SlashCommand("scene-challenge", "Create a scene challenge for extended non-combat scenes against threats or other characters (p. 235).")]
    public async Task BuildSceneChallenge(
      [Summary(description: "The scene challenge's objective.")]
    string title,
      [Summary(description: "The number of clock segments. Default = 6, severe disadvantage = 4, strong advantage = 8.")]
    ClockSize segments=ClockSize.Six,
      [Summary(description: "An optional description.")]
    string description = "",
      [Summary(description: "A score to pre-set the track, if desired.")] [MinValue(0)][MaxValue(10)]
    int score = 0)
    {
        SceneChallenge sceneChallenge = new(Emotes, Context.Interaction.User.Id, Moves, DataFactory, segments, 0, title: title, description: description);
        
        DbContext.ProgressTrackers.Add(sceneChallenge.TrackData);
        await DbContext.SaveChangesAsync();

        await sceneChallenge.EntityAsResponse(RespondAsync);
    }
}

public class CounterComponents : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    public IEmoteRepository Emotes { get; set; }
    public ApplicationContext DbContext { get; set; }

    [ComponentInteraction("clock-advance")]
    public async Task AdvanceClock()
    {
        var clock = Clock.FromEmbed(DbContext, Context.Interaction.Message.Embeds.FirstOrDefault(), Emotes);
        clock.Filled++;

        await Context.Interaction.UpdateAsync(async msg =>
        {
            msg.Components = (await clock.GetComponentsAsync())?.Build();
            msg.Embed = clock.GetEmbed()?.Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("clock-advance:*")]
    public async Task AdvanceClock(string oddsString)
    {
        if (!Enum.TryParse(oddsString, out AskOption odds))
        {
            throw new Exception($"Unable to parse odds from {oddsString}");
        }
        var clock = Clock.FromEmbed(DbContext, Context.Interaction.Message.Embeds.FirstOrDefault(), Emotes);

        OracleAnswer answer = new(Random.Shared, odds, $"Does the clock *{clock.Title}* advance?");
        EmbedBuilder answerEmbed = answer.ToEmbed();
        string resultString = "";
        if (answer.IsYes)
        {
            clock.Filled += answer.IsMatch ? 2 : 1;
            resultString = answer.IsMatch ? $"The clock advances **twice** to {clock.Filled}/{clock.Segments}." : $"The clock advances to {clock.Filled}/{clock.Segments}.";
            answerEmbed = answerEmbed.WithThumbnailUrl(Clock.Images[clock.Segments][clock.Filled]);
            if (answer.IsMatch)
            {
                answerEmbed.WithFooter("You rolled a match! Envision how this situation or project gains dramatic support or inertia.");
            }
        }
        if (!answer.IsYes)
        {
            resultString = $"The clock remains at {clock.Filled}/{clock.Segments}.";
            if (answer.IsMatch)
            {
                answerEmbed = answerEmbed.WithFooter("You rolled a match! Envision a surprising turn of events which pits new factors or forces against the clock.");
            }
        }
        answerEmbed.AddField("Result", resultString);
        answerEmbed = answerEmbed
            .WithUrl(Context.Interaction.Message.GetJumpUrl())
            .WithThumbnailUrl(Clock.Images[clock.Segments][clock.Filled])
            .WithColor(Clock.ColorRamp[clock.Segments][clock.Filled]);

        await Context.Interaction.Message.ModifyAsync(msg =>
        {
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
        
        await Context.Interaction.RespondAsync(embed: answerEmbed.Build()).ConfigureAwait(false);
    }

    [ComponentInteraction("clock-reset")]
    public async Task ResetClock()
    {
        var clock = Clock.FromEmbed(DbContext, Context.Interaction.Message.Embeds.FirstOrDefault(), Emotes);
        clock.Filled = 0;

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Components = clock.MakeComponents().Build();
            msg.Embed = clock.ToEmbed().Build();
        }).ConfigureAwait(false);
    }

    [ComponentInteraction("clock-menu")]
    public async Task ClockMenu(string[] values)
    {
        if (values.Length == 0) return;

        if (values[0].StartsWith("clock-advance:"))
        {
            var oddsString = values[0].Split(":")[1];
            await AdvanceClock(oddsString).ConfigureAwait(false);
            return;
        }

        switch (values[0])
        {
            case "clock-reset":
                await ResetClock().ConfigureAwait(false);
                return;

            case "clock-advance":
                await AdvanceClock().ConfigureAwait(false);
                return;
        }
    }

    [ComponentInteraction("scene-challenge-menu:*")]
    public async Task SceneChallengeMenu(int trackId, string[] values)
    {
        string optionValue = values[0];
        if (optionValue.StartsWith("progress"))
        {
            var track = await DbContext.ProgressTrackers.FindAsync(trackId);

            var scene = new SceneChallenge(Context.Interaction.Message.Embeds.FirstOrDefault(), track, Emotes);
            
            return;
        }
        if (optionValue.StartsWith("clock"))
        {
            await ClockMenu(values).ConfigureAwait(false);
            return;
        }
        return;
    }
}
