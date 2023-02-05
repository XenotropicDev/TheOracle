using Discord.WebSocket;
using Server.Data;
using Server.GameInterfaces;

namespace TheOracle2.UserContent;

internal class PartyEntity : IDiscordEntity
{
    private readonly IEmoteRepository emotes;
    private readonly PlayerDataFactory dataFactory;

    public PartyEntity(Party party, IEmoteRepository emotes, PlayerDataFactory dataFactory)
    {
        this.Party = party;
        this.emotes = emotes;
        this.dataFactory = dataFactory;
    }

    public bool IsEphemeral { get; set; } = false;
    public string? DiscordMessage { get; set; } = null;
    public Party Party { get; }

    public Task<ComponentBuilder?> GetComponentsAsync() => Task.FromResult(new ComponentBuilder()
            .WithButton("Add Supply", $"add-party-supply-{Party.Id}", row: 0, style: ButtonStyle.Success)
            .WithButton("Subtract Supply", $"lose-party-supply-{Party.Id}", row: 0, style: ButtonStyle.Secondary)
            );

    public async Task<IMessage?> GetDiscordMessage(IInteractionContext context)
    {
        if (Party?.ChannelId == null || Party.MessageId == null) throw new NullReferenceException();
        var channel = (Party.ChannelId == context.Channel.Id) ? context.Channel : await (context.Client as DiscordSocketClient)?.Rest.GetChannelAsync(Party.ChannelId ?? 0) as IMessageChannel;
        return await channel?.GetMessageAsync(Party.MessageId ?? 0);
    }

    public EmbedBuilder? GetEmbed()
    {
        var builder = new EmbedBuilder()
        .WithAuthor($"Party Card")
        .WithThumbnailUrl(Party.ImageUrl)
        .WithTitle(Party.Name)
        .AddField("Supply", Party.Supply, true)        
        ;

        if (Party.Characters.Any()) builder.AddField("Characters", string.Join('\n', Party.Characters.Select(pc => pc.Name)));

        return builder;
    }
}
