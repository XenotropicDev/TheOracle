﻿using Discord;
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

        //Func<Cacheable<IUserMessage, ulong>, Cacheable<IMessageChannel, ulong>, SocketReaction, Task>
        public async Task ReactionEventHandler(Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            var reactionHandler = Service.GetRequiredService<ReactionService>();
            if (!reactionHandler.reactionList.Any(item => item.Emote.IsSameAs(reaction.Emote))) return;

            IUser user = (reaction.User.IsSpecified) ? reaction.User.Value : await Client.Rest.GetUserAsync(reaction.UserId);
            if (user.IsBot) return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a handled reaction {reaction.Emote}");

            var message = await userMessage.GetOrDownloadAsync();
            var channel = await cachedChannel.GetOrDownloadAsync() as ISocketMessageChannel;

            var processList = reactionHandler.reactionList.Where(react => react.Emote.IsSameAs(reaction.Emote));
            Parallel.ForEach(processList, (item) =>
            {
                _ = item.ReactionAddedEvent.InvokeAsync(message, channel, reaction, user).ConfigureAwait(false);
            });
        }

        public async Task RemovedReactionHandler(Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> cachedChannel, SocketReaction reaction)
        {
            var reactionHandler = Service.GetRequiredService<ReactionService>();
            if (!reactionHandler.reactionRemovedList.Any(item => item.Emote.IsSameAs(reaction.Emote))) return;

            IUser user = (reaction.User.IsSpecified) ? reaction.User.Value : await Client.Rest.GetUserAsync(reaction.UserId);
            if (user.IsBot) return;

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a handled reaction removed event {reaction.Emote}");

            var message = await userMessage.GetOrDownloadAsync();
            var channel = await cachedChannel.GetOrDownloadAsync() as ISocketMessageChannel;

            var processList = reactionHandler.reactionRemovedList.Where(react => react.Emote.IsSameAs(reaction.Emote));
            Parallel.ForEach(processList, (item) =>
            {
                try
                {
                    _ = item.ReactionRemovedEvent.InvokeAsync(message, channel, reaction, user).ConfigureAwait(false);
                }
                catch (Discord.Net.HttpException httpEx)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} Reactions   {user} triggered a {httpEx.GetType()} - {httpEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
        }
    }
}