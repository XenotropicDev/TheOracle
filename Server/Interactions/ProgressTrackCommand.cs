using Discord.Interactions;
using Discord.WebSocket;
using Server.Data;
using Server.DiceRoller;
using Server.DiscordServer;
using Server.GameInterfaces;
using Server.Interactions.Helpers;
using TheOracle2.GameObjects;

namespace TheOracle2;

public enum ProgressTrackType
{
    Vow,
    Expedition,
    Combat,
    Generic
}


[Group("progress-track", "Create a progress track. For simple progress rolls, use /roll. For scene challenges, use /clock.")]
public class ProgressTrackCommand : InteractionModuleBase
{
    private readonly IEmoteRepository emotes;
    private readonly IMoveRepository moves;
    private readonly PlayerDataFactory playerDataFactory;

    public ApplicationContext DbContext { get; set; }
    public Random Random { get; }

    public ProgressTrackCommand(ApplicationContext dbContext, Random random, IEmoteRepository emotes, IMoveRepository moves, PlayerDataFactory playerDataFactory)
    {
        DbContext = dbContext;
        Random = random;
        this.emotes = emotes;
        this.moves = moves;
        this.playerDataFactory = playerDataFactory;
    }

    //[SlashCommand("vow", "Create a vow progress track for the Swear an Iron Vow move.")]
    //public async Task BuildVowTrack(
    //[Summary(description: "The vow's objective.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //)
    //{
    //    VowTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    //[SlashCommand("expedition", "Create an expedition progress track for the Undertake an Expedition move.")]
    //public async Task BuildExpeditionTrack(
    //[Summary(description: "The expedition's name.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //  )
    //{
    //    ExpeditionTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    //[SlashCommand("combat", "Create a combat progress track when you Enter the Fray.")]
    //public async Task BuildCombatTrack(
    //[Summary(description: "The combat objective.")]
    //string title,
    //[Summary(description: "The challenge rank of the progress track.")]
    //ChallengeRank rank,
    //[Summary(description: "An optional description.")]
    //string description="",
    //[Summary(description: "A score to pre-set the track, if desired.")]
    //[MinValue(0)][MaxValue(10)]
    //int score = 0
    //)
    //{
    //    CombatTrack track = new(dbContext: DbContext, rank: rank, ticks: score * ITrack.BoxSize, title: title, description: description);
    //    await RespondAsync(embed: track.ToEmbed().Build(), components: track.MakeComponents().Build());
    //}

    [SlashCommand("generic", "Create a generic progress track.")]
    public async Task BuildGenericTrack(
    [Summary(description: "A title for the progress track.")]
    string title,
    [Summary(description: "The challenge rank of the progress track.")]
    ChallengeRank rank,
    [Summary(description: "An optional description.")]
    string description="",
    [Summary(description: "A score to pre-set the track, if desired.")][MinValue(0)][MaxValue(10)]
    int score = 0)
    {
        //await DeferAsync();
        ProgressTrack track = new(Random, rank, emotes, moves, playerDataFactory, Context.User.Id, title, description, score);        
        DbContext.ProgressTrackers.Add(track.TrackData);
        await DbContext.SaveChangesAsync();

        await RespondAsync(embeds: track.AsEmbedArray(), components: track.GetComponents()?.Build());
    }
}

public class TrackInteractions : InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>> //InteractionModuleBase<SocketInteractionContext<SocketMessageComponent>>
{
    private readonly Random random;
    private readonly IEmoteRepository emotes;
    private readonly IMoveRepository moves;
    private readonly PlayerDataFactory playerDataFactory;

    public TrackInteractions(ApplicationContext db, Random random, IEmoteRepository emotes, IMoveRepository moves, PlayerDataFactory playerDataFactory)
    {
        Db = db;
        this.random = random;
        this.emotes = emotes;
        this.moves = moves;
        this.playerDataFactory = playerDataFactory;
    }

    public ApplicationContext Db { get; }

    [ComponentInteraction("track-increase-*")]
    public async Task TrackIncrease(int trackId)
    {
        var trackData = Db.ProgressTrackers.Find(trackId);
        if (trackData == null) throw new ArgumentException("Progress track not found");

        trackData.Ticks += trackData.Rank.GetStandardTickAmount();

        ITrack track = new ProgressTrack(trackData, random, emotes, moves, playerDataFactory);

        await Context.Interaction.UpdateAsync(msg =>
        {
            msg.Embeds = track.AsEmbedArray();

        }).ConfigureAwait(false);

        await Db.SaveChangesAsync();
    }

    [ComponentInteraction("progress-main-*")]
    public async Task TrackIncreaseTest(string arg1, string[] selectedRoles)
    {
        if (!int.TryParse(arg1, out int id)) throw new ArgumentException("Progress track not found");
        foreach (var role in selectedRoles)
        {
            if (role == "track-increase") await TrackIncrease(id);
            if (role == "track-roll") await TrackRoll(id);
        }
    }

    [ComponentInteraction("track-roll-*")]
    public async Task TrackRoll(int trackId)
    {
        var trackData = Db.ProgressTrackers.Find(trackId);
        if (trackData == null) throw new ArgumentException("Progress track not found");

        ITrack track = new ProgressTrack(trackData, random, emotes, moves, playerDataFactory);

        var roll = track.Roll();

        await roll.EntityAsResponse(RespondAsync).ConfigureAwait(false);

        await Db.SaveChangesAsync();
    }
}
