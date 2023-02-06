using Discord.WebSocket;
using Server.Data;
using Server.DiceRoller;
using TheOracle2.GameObjects;

namespace TheOracle2.UserContent;

internal class PlayerCharacterEntity : IDiscordEntity
{
    private readonly IEmoteRepository emotes;
    private readonly PlayerDataFactory dataFactory;

    public PlayerCharacterEntity(PlayerCharacter Pc, IEmoteRepository emotes, PlayerDataFactory dataFactory)
    {
        this.Pc = Pc;
        this.emotes = emotes;
        this.dataFactory = dataFactory;
    }

    public bool IsEphemeral { get; set; } = false;
    public string? DiscordMessage { get; set; } = null;
    public PlayerCharacter Pc { get; }

    public Task<ComponentBuilder?> GetComponentsAsync() => Task.FromResult(new ComponentBuilder()
            .WithButton("+Hp", $"add-health-{Pc.Id}", row: 0, style: ButtonStyle.Success)
            .WithButton("-Hp", $"lose-health-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
            .WithButton("+Sp", $"add-spirit-{Pc.Id}", row: 0, style: ButtonStyle.Success)
            .WithButton("-Sp", $"lose-spirit-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
            .WithButton("+Su", $"add-supply-{Pc.Id}", row: 0, style: ButtonStyle.Success)
            .WithButton("-Su", $"lose-supply-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
            .WithButton("+Mo", $"add-momentum-{Pc.Id}", row: 0, style: ButtonStyle.Success)
            .WithButton("-Mo", $"lose-momentum-{Pc.Id}", row: 1, style: ButtonStyle.Secondary)
            .WithButton("Burn", $"burn-momentum-{Pc.Id}", row: 0, style: ButtonStyle.Danger, emote: emotes.BurnMomentum)
            .WithButton("...", $"player-more-{Pc.Id}", row: 0, style: ButtonStyle.Primary));

    public async Task<IMessage?> GetDiscordMessage(IInteractionContext context)
    {
        var channel = (Pc.ChannelId == context.Channel.Id) ? context.Channel : await (context.Client as DiscordSocketClient)?.Rest.GetChannelAsync(Pc.ChannelId) as IMessageChannel;
        return await channel?.GetMessageAsync(Pc.MessageId);
    }

    public EmbedBuilder? GetEmbed()
    {
        var builder = new EmbedBuilder()
        .WithAuthor($"Player Card")
        .WithTitle(Pc.Name)
        .WithThumbnailUrl(Pc.Image)
        .AddField("Stats", $"Edge: {Pc.Edge}, Heart: {Pc.Heart}, Iron: {Pc.Iron}, Shadow: {Pc.Shadow}, Wits: {Pc.Wits}")
        .AddField("Health", Pc.Health, true)
        .AddField("Spirit", Pc.Spirit, true)
        .AddField("Supply", Pc.Supply, true)
        .AddField("Momentum", Pc.Momentum, true)
        .AddField("XP", Pc.XpGained);

        if (Pc.Impacts.Count > 0)
            builder.AddField("Impacts", String.Join(", ", Pc.Impacts));

        return builder;
    }

    /// <summary>
    /// Make an action roll using one of this PC's stats.
    /// </summary>
    public IActionRoll RollAction(Random random, IEmoteRepository emotes, RollableStat stat, int adds, string description = "", int? actionDie = null, int? challengeDie1 = null, int? challengeDie2 = null, string moveName = "")
    {
        ActionRollRandom roll = new ActionRollRandom(random: random, 
            emotes: emotes,
            stat: GetStatValue(stat),
            pdf: dataFactory, 
            db: null,
            PlayerId: Pc.UserId,
            adds: adds,
            momentum: Pc.Momentum,
            description: description,
            actionDie: actionDie,
            challengeDie1: challengeDie1,
            challengeDie2: challengeDie2,
            jumpURL: Pc.GetAssumedJumpUrl()
            //moveName: moveName,
            //pcName: Pc.Name,
            //statName: stat.ToString()
            );
        return roll;
    }

    private int GetStatValue(RollableStat stat)
    {
        return stat switch
        {
            RollableStat.Edge => Pc.Edge,
            RollableStat.Heart => Pc.Heart,
            RollableStat.Iron => Pc.Iron,
            RollableStat.Shadow => Pc.Shadow,
            RollableStat.Wits => Pc.Wits,
            RollableStat.Health => Pc.Health,
            RollableStat.Spirit => Pc.Spirit,
            RollableStat.Supply => Pc.Supply,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
