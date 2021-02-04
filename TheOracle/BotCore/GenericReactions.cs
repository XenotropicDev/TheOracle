using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.GameCore.Oracle;

namespace TheOracle.BotCore
{
    public class GenericReactions
    {
        public const string pinPostEmoji = "📌";
        public const string recreatePostEmoji = "⏬";

        public static Emoji oneEmoji = new Emoji("1️⃣");
        public static Emoji twoEmoji = new Emoji("2️⃣");
        public static Emoji threeEmoji = new Emoji("3️⃣");
        public static Emoji fourEmoji = new Emoji("4️⃣");
        public static Emoji fiveEmoji = new Emoji("5️⃣");

        private TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

        public GenericReactions(IServiceProvider service)
        {
            Service = service;
            Client = Service.GetRequiredService<DiscordSocketClient>();
        }

        public DiscordSocketClient Client { get; }

        public InteractiveService Interactive { get; }

        public IServiceProvider Service { get; }

        public void Load()
        {
            var reactionService = Service.GetRequiredService<ReactionService>();

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

        private async Task addThumbnail(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            CommandContext context = new CommandContext(Client, message);

            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            var embed = message.Embeds.FirstOrDefault();
            if (embed != default)
            {
                var helper = await channel.SendMessageAsync(GenericCommandResources.AddThumbnailHelperMessage).ConfigureAwait(false);
                var urlMessage = await reaction.Channel.NextChannelMessageAsync(client, user: user, timeout: TimeSpan.FromSeconds(20));
                if (urlMessage != null)
                {
                    var builder = embed.ToEmbedBuilder().WithThumbnailUrl(urlMessage.Content);
                    await message.ModifyAsync(msg => msg.Embed = builder.Build());
                    await urlMessage.DeleteAsync().ConfigureAwait(false);
                }
                await helper.DeleteAsync().ConfigureAwait(false);
            }
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

        private async Task deletePostStart(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            await message.AddReactionAsync(new Emoji("☑️"));
        }

        private async Task movePostDown(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            await Task.Run(async () =>
            {
                if (message.Embeds.Count == 0) return;

                if (message.Embeds.Any(NeedsWarning) && !(channel is IDMChannel)) await channel.SendMessageAsync($"{user} moved the following message to the bottom of chat from a message posted on {message.Timestamp.ToLocalTime()}");

                var reactionsToAdd = message.Reactions.Where(item => item.Value.IsMe).Select(item => item.Key);
                var newMessage = await channel.SendMessageAsync(message.Content, message.IsTTS, message.Embeds.FirstOrDefault() as Embed);

                foreach (var reaction in reactionsToAdd)
                {
                    await newMessage.AddReactionAsync(reaction);
                    await Task.Delay(300); //Manual delay to avoid the rate limiter
                }

                await message.DeleteAsync();
            }).ConfigureAwait(false);

            return;
        }

        private bool NeedsWarning(IEmbed embed)
        {
            if (embed.Title.Contains(OracleResources.OracleResult)) return true;

            return false;
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

        private async Task unpinMessage(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var client = Service.GetRequiredService<DiscordSocketClient>();
            if (message.Author.Id != client.CurrentUser.Id) return;

            if (message.IsPinned)
            {
                await message.UnpinAsync().ConfigureAwait(false);
            }
        }
    }
}