using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.GameCore.Oracle;

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
            
            if (channel as IDMChannel != null || reaction.User.Value.IsBot || message.Author.Id != Client.CurrentUser.Id) return;

            if (reaction.Emote.Name == "⏬")
            {
                _ = Task.Run(async () =>
                {
                    if (message.Embeds.Count == 0) return;

                    if (message.Embeds.Any(NeedsWarning)) await channel.SendMessageAsync($"{reaction.User} moved the following message to the bottom of chat from a message posted on {message.Timestamp.ToLocalTime()}");

                    var reactionsToAdd = message.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
                    var newMessage = await channel.SendMessageAsync(message.Content, message.IsTTS, message.Embeds.FirstOrDefault() as Embed);

                    await newMessage.AddReactionsAsync(reactionsToAdd.ToArray()).ConfigureAwait(false);
                    await message.DeleteAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }

            return;
        }

        private bool NeedsWarning(IEmbed embed)
        {
            if (embed.Title.Contains(OracleResources.OracleResult)) return true;

            return false;
        }
    }
}