using System.Text.RegularExpressions;
using Discord.Interactions;
using Discord.WebSocket;
using Server.Data;
using Server.DiceRoller;
using Server.DiscordServer;
using Server.Interactions.Helpers;

namespace TheOracle2;

[Group("roll", "Make an action roll (p. 28) or progress roll (p. 39). For oracle tables, use '/oracle'")]
public class RollCommandGroup : InteractionModuleBase
{
    private readonly IEmoteRepository emotes;
    private readonly ApplicationContext db;
    private readonly PlayerDataFactory dataFactory;

    public RollCommandGroup(Random random, IEmoteRepository emotes, ApplicationContext db, PlayerDataFactory dataFactory)
    {
        Random = random;
        this.emotes = emotes;
        this.db = db;
        this.dataFactory = dataFactory;
    }

    public Random Random { get; }

    [SlashCommand("pc-action", "Make an action roll (p. 28) using a player character's stats.")]
    public async Task ActionRoll(
    [Summary(description: "The character to use for the roll")][Autocomplete(typeof(CharacterAutocomplete))] string character,
    [Summary(description: "The stat value to use for the roll")] RollableStat stat,
    [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
    [Summary(description: "Any notes, fiction, or other text you'd like to include with the roll")] string description = "",
    [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
    [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
    [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null)
    {
        if (!int.TryParse(character, out int id))
        {
            await RespondAsync($"Unknown character", ephemeral: true);
            return;
        }

        var pc = db.PlayerCharacters.Find(id);
        if (pc == null)
        {
            await RespondAsync($"Unknown character", ephemeral: true);
            return;
        }

        var roll = new ActionRollRandom(Random, emotes, dataFactory, db, Context.User.Id, pc.GetStat(stat, db), adds, pc.GetStat(RollableStat.Momentum, db), description, actionDie, challengeDie1, challengeDie2, id, statName: stat.ToString());

        await roll.EntityAsResponse(RespondAsync).ConfigureAwait(false);
    }

    [SlashCommand("action", "Make an action roll (p. 28) by setting the stat values. [pc-action roll is recommended over this]")]
    public async Task RollAction(
        [Summary(description: "The stat value to use for the roll")] int stat,
        [Summary(description: "Any adds to the roll")][MinValue(0)] int adds,
        [Summary(description: "The player character's momentum.")][MinValue(-6)][MaxValue(10)] int momentum = 0,
        [Summary(description: "Any notes, fiction, or other text you'd like to include with the roll")] string description = "",
        [Summary(description: "A preset value for the Action Die (d6) to use instead of rolling.")][MinValue(1)][MaxValue(6)] int? actionDie = null,
        [Summary(description: "A preset value for the first Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die (d10) to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null)
    {
        var roll = new ActionRollRandom(Random, emotes, dataFactory, db, Context.User.Id, stat, adds, momentum, description, actionDie, challengeDie1, challengeDie2);
        await roll.EntityAsResponse(RespondAsync).ConfigureAwait(false);
    }

    [SlashCommand("progress", "Roll with a set progress score (p. 39). For an interactive progress tracker, use /progress-track.")]
    public async Task RollProgress(
        [Summary(description: "The progress score.")] int progressScore,
        [Summary(description: "A preset value for the first Challenge Die to use instead of rolling.")][MinValue(1)][MaxValue(10)] int? challengeDie1 = null,
        [Summary(description: "A preset value for the second Challenge Die to use instead of rolling")][MinValue(1)][MaxValue(10)] int? challengeDie2 = null,
        [Summary(description: "Notes, fiction, or other text to include with the roll.")] string description = "")
    {
        var roll = new ProgressRollRandom(Random, progressScore, description, challengeDie1, challengeDie2);
        await roll.EntityAsResponse(RespondAsync).ConfigureAwait(false);
    }
}

public class RollInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly ApplicationContext db;
    private readonly Random random;
    private readonly IEmoteRepository emotes;
    private readonly DiscordSocketClient client;
    private readonly PlayerDataFactory dataFactory;

    public RollInteractions(ApplicationContext db, Random random, IEmoteRepository emotes, DiscordSocketClient client, PlayerDataFactory dataFactory)
    {
        this.db = db;
        this.random = random;
        this.emotes = emotes;
        this.client = client;
        this.dataFactory = dataFactory;
    }

    private IActionRoll? GetActionRollFromEmbed(IEmbed embed, int? characterId = null)
    {
        if (embed == null) return null;
        var builder = embed.ToEmbedBuilder();

        if (builder.Fields.FirstOrDefault(f => f.Name == "Action Score")?.Value is not string scoreField) return null;
        if (builder.Fields.FirstOrDefault(f => f.Name == "Challenge Dice")?.Value is not string challengeField) return null;

        scoreField = scoreField[..scoreField.IndexOf("=")];
        var match = Regex.Match(scoreField, @"(\d+)\D+(\d+)\D+(\d+)");
        if (!match.Success) return null;

        int.TryParse(match.Groups[1].Value, out int d6);
        int.TryParse(match.Groups[2].Value, out int stat);
        int.TryParse(match.Groups[3].Value, out int adds);

        match = Regex.Match(challengeField, @"(\d+)\D+(\d+)");
        if (!match.Success) return null;

        int.TryParse(match.Groups[1].Value, out int challengeDie1);
        int.TryParse(match.Groups[2].Value, out int challengeDie2);

        string? rolledStat = embed.Author.HasValue ? embed.Author.Value.Name.Substring(embed.Author.Value.Name.IndexOf("+")) : null;

        return new ActionRollRandom(random, emotes, dataFactory, db, Context.User.Id, stat, adds, null, builder.Description, d6, challengeDie1, challengeDie2, characterId, statName: rolledStat);
    }

    [ComponentInteraction("burn-roll:*,*")]
    public async Task BurnRoll(int momentum, int characterId)
    {
        var getPcTask = db.PlayerCharacters.FindAsync(characterId).ConfigureAwait(false);
        var actionRoll = GetActionRollFromEmbed(Context.Interaction.Message.Embeds.FirstOrDefault()!, characterId);
        if (actionRoll == null || actionRoll is not IBurnable burnableAction) return;

        burnableAction.Burn(momentum, null);

        var updateRollTask = Context!.Interaction.UpdateAsync(async msg =>
        {
            msg.Embeds = actionRoll.AsEmbedArray();
            msg.Components = await actionRoll.AsMessageComponent();
        }).ConfigureAwait(false);

        var pc = await getPcTask;
        if (pc != null)
        {
            pc.BurnMomentum();
            await db.SaveChangesAsync().ConfigureAwait(false);
            await pc.UpdateCardDisplay(client, emotes, dataFactory).ConfigureAwait(false);
        };

        await updateRollTask;
    }
}
