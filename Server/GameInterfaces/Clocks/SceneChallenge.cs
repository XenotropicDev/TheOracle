using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Server.Data;
using Server.DiceRoller;
using Server.DiscordServer;
using Server.GameInterfaces;
using static System.Formats.Asn1.AsnWriter;

namespace TheOracle2.GameObjects;

public class SceneChallenge : Clock, ITrack
{
    [SetsRequiredMembers]
    public SceneChallenge(Embed embed, IEmoteRepository emotes) : base(embed)
    {
        Emotes = emotes;
    }

    [SetsRequiredMembers]
    public SceneChallenge(IEmoteRepository emotes, ulong playerId, IMoveRepository moves, PlayerDataFactory dataFactory, ClockSize segments = ClockSize.Six, int filledSegments = 0, int ticks = 0, string title = "", string description = "", ChallengeRank rank = ChallengeRank.Formidable) : base(segments, filledSegments, title, description)
    {
        Emotes = emotes;
        Rank = rank;

        TrackData = new TrackData
        {
            Rank = rank,
            Ticks = ticks,
            Title = title,
            Description = description,
            PlayerId = playerId
        };

        ProgressTrack = new ProgressTrack(TrackData, Random.Shared, emotes, moves, dataFactory);
    }

    public override string EmbedCategory => "Scene Challenge";
    public string FooterMessage { get; set; } = "When the tension clock is filled, time is up. You must resolve the encounter by making a progress roll.";
    public string ResolveMoveName => "Resolve Scene Challenge";
    public string TrackDescription => "Scene Challenge";
    public bool CanRecommit => false;
    public string MarkAlertTitle => "Mark Progress";
    public override string ClockFillMessage => "Time is up. You must resolve the encounter by making a progress roll.";

    public SelectMenuBuilder MakeSelectMenu()
    {
        SelectMenuBuilder menu = new SelectMenuBuilder()
            .WithPlaceholder($"Manage {TrackDescription.ToLowerInvariant()}...")
            .WithCustomId($"scene-challenge-menu:{TrackData.Id}")
            .WithMaxValues(1)
            .WithMinValues(0)
        ;

        if (!IsFull)
        {
            if (GetScore() < TrackSize) 
            { 
                string alertLabel = MarkAlertTitle;
                string description = $"Mark {this.TickString(Rank.GetStandardTickAmount())} of progress";
                SelectMenuOptionBuilder option = new SelectMenuOptionBuilder().WithEmote(Emotes.ProgressEmotes[(int)Rank - 1]).WithValue($"progress-mark:{Rank.GetStandardTickAmount()}");
                if (string.IsNullOrEmpty(alertLabel)) { option.WithLabel(description); }
                if (!string.IsNullOrEmpty(alertLabel))
                {
                    option
                      .WithLabel(alertLabel)
                      .WithDescription(description);
                }
                menu.AddOption(option);
            }

            menu.AddOption(AdvanceOption());
        }

        SelectMenuOptionBuilder resolveOption = new SelectMenuOptionBuilder()
            .WithEmote(Emotes.Roll)
            .WithLabel(ResolveMoveName)
            .WithDescription("Roll progress")
            .WithValue($"progress-roll");
        
        menu.AddOption(resolveOption);

        if (Ticks > 0) 
        {
            menu.AddOption(new SelectMenuOptionBuilder()
            .WithLabel($"Clear {this.TickString(Rank.GetStandardTickAmount())} of progress")
            .WithEmote(Emotes.ProgressEmotes[0])
            .WithValue($"progress-clear:{Rank.GetStandardTickAmount()}"));
        }
        
        if (Filled > 0 || Ticks > 0) { menu.AddOption(ResetOption()); }
        
        return menu;
    }

    public IActionRoll Roll()
    {
        return new ProgressRollRandom(Random.Shared, GetScore(), $"Scene Challenge Roll for {TrackData.Title}");
    }

    public new EmbedBuilder? GetEmbed()
    {
        var builder = ProgressTrack.GetEmbed();

        if (builder != null) AddClockTemplate(builder, this);

        return builder;
    }

    public new async Task<ComponentBuilder?> GetComponentsAsync()
    {
        return (await ProgressTrack.GetComponentsAsync())?.WithSelectMenu(MakeSelectMenu());
    }

    public int GetScore()
    {
        int rawScore = Ticks / BoxSize;
        return Math.Min(rawScore, TrackSize);
    }

    public TrackData TrackData { get; set; }
    public int BoxSize { get; set; } = 4;
    public new bool IsEphemeral { get; set; } = false;
    public new string? DiscordMessage { get; set; } = null;
    public string TrackDisplayName { get; set; } = "Track";
    public int TrackSize { get; set; } = 10;
    public int Ticks { get => TrackData.Ticks; set => TrackData.Ticks = value; }
    public int MaxTicks { get; set; }
    public ChallengeRank Rank { get; }
    public ProgressTrack ProgressTrack { get; }
    public IEmoteRepository Emotes { get; }
}
