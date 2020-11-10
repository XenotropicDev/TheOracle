﻿using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.GameCore.Oracle;

namespace TheOracle.BotCore
{
    public class GenericReactions
    {
        public const string recreatePostEmoji = "⏬";
        public const string pinPostEmoji = "📌";

        public GenericReactions(IServiceProvider service)
        {
            Service = service;
            var reactionService = service.GetRequiredService<ReactionService>();

            ReactionEvent moveDownReaction = new ReactionEventBuilder().WithEmoji(recreatePostEmoji).WithEvent(movePostDown).Build();
            ReactionEvent deleteReaction = new ReactionEventBuilder().WithEmoji("❌").WithEvent(deletePostStart).Build();
            ReactionEvent confrimDeleteReaction = new ReactionEventBuilder().WithEmoji("☑️").WithEvent(deletePostConfirm).Build();

            ReactionEvent pinMessageReaction = new ReactionEventBuilder().WithEmoji(pinPostEmoji).WithEvent(pinMessage).Build();
            ReactionEvent unpinMessageReaction = new ReactionEventBuilder().WithEmoji(pinPostEmoji).WithRemoveEvent(unpinMessage).Build();

            reactionService.reactionList.Add(moveDownReaction);
            reactionService.reactionList.Add(deleteReaction);
            reactionService.reactionList.Add(confrimDeleteReaction);
            reactionService.reactionList.Add(pinMessageReaction);

            reactionService.reactionRemovedList.Add(unpinMessageReaction);
        }

        private async Task unpinMessage(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            if (message.IsPinned)
            {
                await message.UnpinAsync().ConfigureAwait(false);
            }
        }

        private async Task pinMessage(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            if (!message.IsPinned)
            {
                await message.PinAsync().ConfigureAwait(false);
            }
        }

        public IServiceProvider Service { get; }

        private async Task movePostDown(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            await Task.Run(async () =>
            {
                if (message.Embeds.Count == 0) return;

                if (message.Embeds.Any(NeedsWarning) && !(channel is IDMChannel)) await channel.SendMessageAsync($"{user} moved the following message to the bottom of chat from a message posted on {message.Timestamp.ToLocalTime()}");

                var reactionsToAdd = message.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
                var newMessage = await channel.SendMessageAsync(message.Content, message.IsTTS, message.Embeds.FirstOrDefault() as Embed);

                await newMessage.AddReactionsAsync(reactionsToAdd.ToArray()).ConfigureAwait(false);
                await message.DeleteAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            return;
        }

        private async Task deletePostStart(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            await message.AddReactionAsync(new Emoji("☑️"));
        }

        private async Task deletePostConfirm(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            if (message.Reactions.ContainsKey(new Emoji("❌")))
            {
                await message.DeleteAsync();
            }
        }

        private bool NeedsWarning(IEmbed embed)
        {
            if (embed.Title.Contains(OracleResources.OracleResult)) return true;

            return false;
        }
    }
}