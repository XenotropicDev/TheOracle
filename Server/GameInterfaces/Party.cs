using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Interactions.Helpers;
using TheOracle2;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace Server.GameInterfaces;

public class Party
{
    private int supply = 5;

    public int Id { get; set; }
    public string Name { get; set; }
    public List<PlayerCharacter> Characters { get; set; } = new();
    public int Supply { get => supply; set => supply = (value <= 5 && value >= 0) ? value : supply; }
    public string? ImageUrl { get; set; }
    public required ulong? DiscordGuildId { get; set; }
    public required ulong? MessageId { get; set; }
    public required ulong? ChannelId { get; set; }

    internal async Task UpdateCardDisplay(IDiscordClient discordSocketClient, IEmoteRepository emotes, PlayerDataFactory dataFactory)
    {
        var msg = await GetPartyMessage(discordSocketClient).ConfigureAwait(false);
        if (msg == null) return;

        var entity = new PartyEntity(this, emotes, dataFactory);
        await msg.ModifyAsync(msg => msg.Embeds = entity.AsEmbedArray()).ConfigureAwait(false);
    }

    public async Task<IUserMessage?> GetPartyMessage(IDiscordClient client)
    {
        if (await client.GetChannelAsync(ChannelId ?? 0).ConfigureAwait(false) is not IMessageChannel channel) return null;

        return await channel.GetMessageAsync(MessageId ?? 0).ConfigureAwait(false) as IUserMessage;
    }
}
