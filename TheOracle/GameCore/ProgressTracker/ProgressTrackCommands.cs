using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.Action;
using TheOracle.GameCore.ProgressTracker;

namespace TheOracle.GameCore.ProgressTracker
{
    public class ProgressTrackCommands : ModuleBase<SocketCommandContext>
    {
        public const string DecreaseEmoji = "\u25C0";
        public const string fiveEmoji = "\u0035\u20E3";
        public const string fourEmoji = "\u0034\u20E3";
        public const string FullEmoji = "\u2714";
        public const string IncreaseEmoji = "\u25B6";
        public const string oneEmoji = "\u0031\u20E3";
        public const string RollEmoji = "\uD83C\uDFB2";
        public const string threeEmoji = "\u0033\u20E3";
        public const string twoEmoji = "\u0032\u20E3";

        public ProgressTrackCommands(IServiceProvider service)
        {
            Service = service;
            Client = service.GetRequiredService<DiscordSocketClient>();
            var hooks = service.GetRequiredService<HookedEvents>();

            if (!hooks.ProgressReactions)
            {
                hooks.ProgressReactions = true;
                var reactionService = Service.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmoji(oneEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmoji(twoEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmoji(threeEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmoji(fourEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmoji(fiveEmoji).WithEvent(ProgressBuilderReactions).Build();

                ReactionEvent decrease = new ReactionEventBuilder().WithEmoji(DecreaseEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent increase = new ReactionEventBuilder().WithEmoji(IncreaseEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent fullMark = new ReactionEventBuilder().WithEmoji(FullEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent roll = new ReactionEventBuilder().WithEmoji(RollEmoji).WithEvent(ProgressInteractiveReactions).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);
                reactionService.reactionList.Add(reaction5);

                reactionService.reactionList.Add(decrease);
                reactionService.reactionList.Add(increase);
                reactionService.reactionList.Add(fullMark);
                reactionService.reactionList.Add(roll);
            }
        }

        public DiscordSocketClient Client { get; }
        public IServiceProvider Service { get; }

        public async Task ProgressBuilderReactions(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsProgressTrackerMessage(message)) return;

            string ThingToTrack = message.Embeds.FirstOrDefault(embed => embed.Title == ProgressResources.Progress_Tracker)?.Description ?? "Unknown Task";

            await Task.Run(async () =>
            {
                if (reaction.Emote.Name == oneEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Troublesome, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == twoEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Dangerous, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == threeEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Formidable, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == fourEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Extreme, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == fiveEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Epic, ThingToTrack, message).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return;
        }

        public async Task ProgressInteractiveReactions(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!ProgressTracker.IsProgressTrackerMessage(message)) return;

            if (reaction.Emote.Name == DecreaseEmoji)
            {
                DecreaseProgress(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.Name == IncreaseEmoji)
            {
                IncreaseProgress(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.Name == FullEmoji)
            {
                IncreaseProgressFullCheck(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.Name == RollEmoji)
            {
                var tracker = new ProgressTracker(message);
                var roll = new ActionRoll(0, tracker.ActionDie, $"{ProgressResources.ProgressRollFor}{tracker.Title}");
                await channel.SendMessageAsync(roll.ToString()).ConfigureAwait(false);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }

            return;
        }

        [Command("ProgressTracker")]
        [Alias("Track", "Tracker", "Progress")]
        [Summary("Creates an objective tracking post for things like Iron vows, journeys, and combat encounters")]
        [Remarks("\u25C0 - Decreases the progress track by the difficulty amount.\n\u25B6 - Increases the progress track by the difficulty amount.\n\u2714 - Increases the progress track by a single full box (four ticks).\n\uD83C\uDFB2 - Rolls the action and challenge die for the progress tracker.")]
        public async Task ProgressTrackerCommand([Remainder] string TrackerArgs)
        {
            //TODO this all needs to be reworked for globalization
            string[] splitArgs = TrackerArgs.Split(' ');
            string NameOfTrack = TrackerArgs;
            ChallengeRank difficulty = ChallengeRank.None;

            if (Enum.TryParse(Enum.GetNames(typeof(ChallengeRank)).FirstOrDefault(cr => ProgressTracker.HasMatchingChallengeRank(cr, splitArgs)), out difficulty))
            {
                NameOfTrack = Regex.Replace(NameOfTrack, difficulty.ToString(), string.Empty, RegexOptions.IgnoreCase);
            }

            if (difficulty == ChallengeRank.None) await CreateEmptyTracker(NameOfTrack);
            else await BuildProgressTrackerPostAsync(difficulty, NameOfTrack);
        }

        private async Task BuildProgressTrackerPostAsync(ChallengeRank cr, string ThingToTrack, IUserMessage messageToEdit = null)
        {
            if (messageToEdit == null)
            {
                var tracker = new ProgressTracker(cr, ThingToTrack);
                messageToEdit = ReplyAsync(embed: tracker.BuildEmbed()).Result;
            }
            else
            {
                var tracker = new ProgressTracker(messageToEdit, cr);
                await messageToEdit.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = tracker.BuildEmbed();
                });
            }

            await messageToEdit.RemoveAllReactionsAsync();

            _ = Task.Run(async () =>
            {
                await messageToEdit.AddReactionAsync(new Emoji(DecreaseEmoji));
                await messageToEdit.AddReactionAsync(new Emoji(IncreaseEmoji));
                await messageToEdit.AddReactionAsync(new Emoji(FullEmoji));
                await messageToEdit.AddReactionAsync(new Emoji(RollEmoji));
                await messageToEdit.AddReactionAsync(new Emoji(GenericReactions.recreatePostEmoji));
            }).ConfigureAwait(false);

            return;
        }

        private async Task CreateEmptyTracker(string nameOfTrack)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(ProgressResources.Progress_Tracker)
                .WithDescription(nameOfTrack)
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Reactions,
                    Value = $"{oneEmoji} = {ProgressResources.Troublesome}" +
                            $"\n{twoEmoji} = {ProgressResources.Dangerous}" +
                            $"\n{threeEmoji} = {ProgressResources.Formidable}" +
                            $"\n{fourEmoji} = {ProgressResources.Extreme}" +
                            $"\n{fiveEmoji} = {ProgressResources.Epic}"
                });

            var msg = await ReplyAsync(embed: embed.Build());

            _ = Task.Run(async () =>
            {
                await msg.AddReactionAsync(new Emoji(oneEmoji));
                await msg.AddReactionAsync(new Emoji(twoEmoji));
                await msg.AddReactionAsync(new Emoji(threeEmoji));
                await msg.AddReactionAsync(new Emoji(fourEmoji));
                await msg.AddReactionAsync(new Emoji(fiveEmoji));
            }).ConfigureAwait(false);

            return;
        }

        private void DecreaseProgress(IUserMessage message)
        {
            ProgressTracker tracker = new ProgressTracker(message);

            tracker.RemoveProgress();

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed()).ConfigureAwait(false);
        }

        private void IncreaseProgress(IUserMessage message)
        {
            ProgressTracker tracker = new ProgressTracker(message);

            tracker.AddProgress();

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed()).ConfigureAwait(false);
        }

        private void IncreaseProgressFullCheck(IUserMessage message)
        {
            ProgressTracker tracker = new ProgressTracker(message);

            tracker.Ticks += 4;

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed()).ConfigureAwait(false);
        }

        private bool IsProgressTrackerMessage(IUserMessage message)
        {
            if (message.Embeds == null) return false;
            if (message.Embeds?.FirstOrDefault()?.Title == ProgressResources.Progress_Tracker) return true;

            return false;
        }
    }
}