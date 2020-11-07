using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle.BotCore
{
    public class GlobalReactionHandler
    {
        public GlobalReactionHandler(IServiceProvider service)
        {
            Service = service;
            Client = service.GetRequiredService<DiscordSocketClient>();
        }

        public IServiceProvider Service { get; }
        private DiscordSocketClient Client { get; }

        public async Task ReactionEventHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var reactionHandler = Service.GetRequiredService<ReactionService>();
            if (!reactionHandler.reactionList.Any(item => item.Emote.Name == reaction.Emote.Name)) return;

            IUser user = (reaction.User.IsSpecified) ? reaction.User.Value : await Client.Rest.GetUserAsync(reaction.UserId);
            if (user.IsBot) return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a handled reaction {reaction.Emote}");

            var message = await userMessage.GetOrDownloadAsync();

            Parallel.ForEach(reactionHandler.reactionList.Where(react => react.Emote.Name == reaction.Emote.Name), (item) =>
            {
                try
                {
                    _ = item.ReactionAddedEvent.InvokeAsync(message, channel, reaction, user);
                }
                catch (Discord.Net.HttpException httpEx)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a {httpEx.GetType()} - {httpEx.Message}");
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }

        public async Task RemovedReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var reactionHandler = Service.GetRequiredService<ReactionService>();
            if (!reactionHandler.reactionRemovedList.Any(item => item.Emote.Name == reaction.Emote.Name)) return;

            IUser user = (reaction.User.IsSpecified) ? reaction.User.Value : await Client.Rest.GetUserAsync(reaction.UserId);
            if (user.IsBot) return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a handled reaction removed event {reaction.Emote}");

            var message = await userMessage.GetOrDownloadAsync();

            Parallel.ForEach(reactionHandler.reactionList.Where(react => react.Emote.Name == reaction.Emote.Name), (item) =>
            {
                try
                {
                    _ = item.ReactionRemovedEvent.InvokeAsync(message, channel, reaction, user);
                }
                catch (Discord.Net.HttpException httpEx)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a {httpEx.GetType()} - {httpEx.Message}");
                }
                catch (Exception)
                {
                    throw;
                }
            });
        }
    }
}