using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.InitiativeTracker
{
    public class InitiativeTrackerCommands : ModuleBase<SocketCommandContext>
    {
        public const string AdvantageEmoji = "\u25C0";
        public const string DisadvantageEmoji = "\u25B6";

        public InitiativeTrackerCommands(ServiceProvider services)
        {
            Services = services;

            var hooks = Services.GetRequiredService<HookedEvents>();
            if (!hooks.InitiativeReactions)
            {
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmoji(AdvantageEmoji).WithEvent(InitiativeReactionsHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmoji(DisadvantageEmoji).WithEvent(InitiativeReactionsHandler).Build();

                services.GetRequiredService<ReactionService>().reactionList.Add(reaction1);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction2);
                hooks.InitiativeReactions = true;
            }
        }

        public ServiceProvider Services { get; }

        [Command("InitiativeTracker")]
        [Alias("Initiative", "IniTracker")]
        public async Task BuildTrackerCommand([Remainder] string Description = "")
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);

            InitiativeTracker tracker = new InitiativeTracker(channelSettings);
            tracker.Description = Description;
            var msg = await ReplyAsync(embed: tracker.GetEmbedBuilder().Build());
            await msg.AddReactionAsync(new Emoji(AdvantageEmoji));
            await msg.AddReactionAsync(new Emoji(DisadvantageEmoji));
            //await msg.AddReactionAsync(new Emoji(DeleteEmoji));

            return;
        }

        private async Task InitiativeReactionsHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(channel.Id);

            if (!InitiativeTracker.IsInitiativeTrackerMessage(message)) return;

            InitiativeTracker tracker = InitiativeTracker.FromMessage(message).WithChannelSettings(channelSettings);
            if (reaction.Emote.Name == DisadvantageEmoji)
            {
                if (!tracker.Disadvantage.Contains(user.ToString()))
                {
                    await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
                    tracker.Disadvantage.Add(user.ToString());
                    tracker.Advantage.RemoveAll(s => s == user.ToString());
                    await message.ModifyAsync(msg => msg.Embed = tracker.GetEmbedBuilder().Build());
                    return;
                }
            }
            if (reaction.Emote.Name == AdvantageEmoji)
            {
                if (!tracker.Advantage.Contains(user.ToString()))
                {
                    await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
                    tracker.Advantage.Add(user.ToString());
                    tracker.Disadvantage.RemoveAll(s => s == user.ToString());
                    await message.ModifyAsync(msg => msg.Embed = tracker.GetEmbedBuilder().Build());
                    return;
                }
            }

            return;
        }
    }
}