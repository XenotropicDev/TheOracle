using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class GlobalReactionHandler
    {
        public GlobalReactionHandler(ServiceProvider service)
        {
            Service = service;
            Client = service.GetRequiredService<DiscordSocketClient>();
        }

        public ServiceProvider Service { get; }
        private DiscordSocketClient Client { get; }

        public async Task ReactionEventHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await userMessage.GetOrDownloadAsync();
            if (reaction.User.Value.IsBot || message.Author.Id != Client.CurrentUser.Id) return;

            if (reaction.Emote.Name == "⏬")
            {
                _ = Task.Run(async () =>
                {
                    var warningMsg = await channel.SendMessageAsync($"{reaction.User} moved the following message to the bottom of chat from a message posted on {message.Timestamp.ToLocalTime()}");

                    var reactionsToAdd = message.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
                    var msg = await channel.SendMessageAsync(message.Content, message.IsTTS, message.Embeds.FirstOrDefault() as Embed);

                    await msg.AddReactionsAsync(reactionsToAdd.ToArray()).ConfigureAwait(false);
                    await message.DeleteAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }

            return;
        }
    }
}