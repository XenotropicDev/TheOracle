using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TheOracle.GameCore.InitiativeTracker
{
    public class InitiativeTrackerCommands : ModuleBase<SocketCommandContext>
    {
        public const string AdvantageEmoji = "\u25C0";
        public const string ConfirmEmoji = "\u2713";
        public const string DeleteEmoji = "\u274C";
        public const string DisadvantageEmoji = "\u25B6";
        public InitiativeTrackerCommands(ServiceProvider services)
        {
            Services = services;

            var hooks = Services.GetRequiredService<HookedEvents>();
            if (!hooks.InitiativeReactions)
            {
                var client = Services.GetRequiredService<DiscordSocketClient>();
                client.ReactionAdded += InitiativeReactionsHandler;
                hooks.InitiativeReactions = true;
            }
        }

        public ServiceProvider Services { get; }

        [Command("InitiativeTracker")]
        [Alias("Initiative", "IniTracker")]
        public async Task BuildTrackerCommand([Remainder] string Description = "")
        {
            InitiativeTracker tracker = new InitiativeTracker();
            tracker.Description = Description;
            var msg = await ReplyAsync(embed: tracker.GetEmbedBuilder().Build());
            await msg.AddReactionAsync(new Emoji(AdvantageEmoji));
            await msg.AddReactionAsync(new Emoji(DisadvantageEmoji));
            //await msg.AddReactionAsync(new Emoji(DeleteEmoji));

            return;
        }

        private async Task InitiativeReactionsHandler(Discord.Cacheable<Discord.IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var emojisToProcess = new Emoji[] { new Emoji(AdvantageEmoji), new Emoji(DisadvantageEmoji), new Emoji(DeleteEmoji), new Emoji(ConfirmEmoji) };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return;

            var message = await userMessage.GetOrDownloadAsync();
            if (!InitiativeTracker.IsInitiativeTrackerMessage(message)) return;

            InitiativeTracker tracker = InitiativeTracker.FromMessage(message);
            if (reaction.Emote.Name == DisadvantageEmoji)
            {
                if (!tracker.Disadvantage.Contains(reaction.User.ToString()))
                {
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value).ConfigureAwait(false);
                    tracker.Disadvantage.Add(reaction.User.ToString());
                    tracker.Advantage.RemoveAll(s => s == reaction.User.ToString());
                    await message.ModifyAsync(msg => msg.Embed = tracker.GetEmbedBuilder().Build());
                    return;
                }
            }
            if (reaction.Emote.Name == AdvantageEmoji)
            {
                if (!tracker.Advantage.Contains(reaction.User.ToString()))
                {
                    await message.RemoveReactionAsync(reaction.Emote, reaction.User.Value).ConfigureAwait(false);
                    tracker.Advantage.Add(reaction.User.ToString());
                    tracker.Disadvantage.RemoveAll(s => s == reaction.User.ToString());
                    await message.ModifyAsync(msg => msg.Embed = tracker.GetEmbedBuilder().Build());
                    return;
                }
            }

            return;
        }
    }
}