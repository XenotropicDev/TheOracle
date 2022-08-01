using Discord.Interactions;
using Discord.WebSocket;
using Server.DiscordServer;
using Server.Interactions;
using TheOracle2.GameObjects;
using TheOracle2.UserContent;

namespace TheOracle2.Commands;

public class RecreateMessageCommand : InteractionModuleBase<SocketInteractionContext<SocketMessageCommand>>
{
    public RecreateMessageCommand(ApplicationContext dbContext)
    {
        DbContext = dbContext;
    }

    public ApplicationContext DbContext { get; }

    [MessageCommand("Recreate Message")]
    public async Task MoveToBottom(IMessage msg)
    {
        if (msg.Author.Id != Context.Client.CurrentUser.Id) await RespondAsync($"I can't recreate that message", ephemeral: true);

        await DeferAsync();

        var builder = ComponentBuilder.FromMessage(msg);
        var content = msg.Content?.Length > 0 ? msg.Content : null;
        
        var newMsg = await FollowupAsync(content, embeds: msg.Embeds.OfType<Embed>().ToArray(), components: builder.Build()).ConfigureAwait(false);

        if (DbContext.PlayerCharacters.FirstOrDefault(pc => pc.MessageId == msg.Id) is PlayerCharacter pc)
        {
            pc.MessageId = newMsg.Id;
            await DbContext.SaveChangesAsync();
        }

        await msg.DeleteAsync().ConfigureAwait(false);
    }
}
