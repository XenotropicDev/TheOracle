using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.InitiativeTracker
{
    public class InitiativeTrackerCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji AdvantageEmoji = new Emoji("\u25C0");
        public Emoji DisadvantageEmoji = new Emoji("\u25B6");

        public InitiativeTrackerCommands(ServiceProvider services)
        {
            Services = services;

            var hooks = Services.GetRequiredService<HookedEvents>();
            if (!hooks.InitiativeReactions)
            {
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(AdvantageEmoji).WithEvent(InitiativeReactionsHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(DisadvantageEmoji).WithEvent(InitiativeReactionsHandler).Build();

                services.GetRequiredService<ReactionService>().reactionList.Add(reaction1);
                services.GetRequiredService<ReactionService>().reactionList.Add(reaction2);
                hooks.InitiativeReactions = true;
            }
        }

        public ServiceProvider Services { get; }

        [Command("InitiativeTracker")]
        [Alias("Initiative", "IniTracker")]
        [Summary("Creates an interactive post for tracking combat initiative")]
        [Remarks("\u25C0 - Adds or moves you to the advantage category\n\u25B6 - Adds or moves you to the disadvantage category")]
        public async Task BuildTrackerCommand([Remainder] string Description = "")
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);

            InitiativeTracker tracker = new InitiativeTracker(channelSettings);
            tracker.Description = Description;
            var msg = await ReplyAsync(embed: tracker.GetEmbedBuilder().Build());
            await msg.AddReactionAsync(AdvantageEmoji);
            await msg.AddReactionAsync(DisadvantageEmoji);

            return;
        }

        private async Task InitiativeReactionsHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(channel.Id);

            if (!InitiativeTracker.IsInitiativeTrackerMessage(message)) return;

            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            InitiativeTracker tracker = InitiativeTracker.FromMessage(message).WithChannelSettings(channelSettings);

            if (reaction.Emote.IsSameAs(DisadvantageEmoji))
            {
                if (!tracker.Disadvantage.Contains(user.ToString()))
                {
                    tracker.Disadvantage.Add(user.ToString());
                    tracker.Advantage.RemoveAll(s => s == user.ToString());
                    await message.ModifyAsync(msg => msg.Embed = tracker.GetEmbedBuilder().Build());
                    return;
                }
            }
            if (reaction.Emote.IsSameAs(AdvantageEmoji))
            {
                if (!tracker.Advantage.Contains(user.ToString()))
                {
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