using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.GameCore.ProgressTracker;

namespace TheOracle.Core
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

        public ProgressTrackCommands(DiscordSocketClient client, HookedEvents hooks)
        {
            Client = client;
            if (!hooks.ProgressReactions)
            {
                hooks.ProgressReactions = true;
                Client.ReactionAdded += ProgressBuilderReactions;
                Client.ReactionAdded += ProgressInteractiveReactions;
            }
        }

        public DiscordSocketClient Client { get; }

        public Task ProgressBuilderReactions(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            //TODO Concurrent queue so that users can't spam reactions?
            var emojisToProcess = new Emoji[] { new Emoji(oneEmoji), new Emoji(twoEmoji), new Emoji(threeEmoji), new Emoji(fourEmoji), new Emoji(fiveEmoji) };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;
            if (!IsProgressTrackerMessage(message)) return Task.CompletedTask;
            
            Console.WriteLine($"User {reaction.User} triggered {nameof(this.ProgressBuilderReactions)} reaction {reaction.Emote.Name}");

            string ThingToTrack = message.Embeds.FirstOrDefault(embed => embed.Title == ProgressResources.Progress_Tracker)?.Description ?? "Unknown Task";

            _ = Task.Run(async () =>
            {
                if (reaction.Emote.Name == oneEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Troublesome, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == twoEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Dangerous, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == threeEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Formidable, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == fourEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Extreme, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.Name == fiveEmoji) await BuildProgressTrackerPostAsync(ChallengeRank.Epic, ThingToTrack, message).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task ProgressInteractiveReactions(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var emojisToProcess = new Emoji[] { new Emoji(DecreaseEmoji), new Emoji(IncreaseEmoji), new Emoji(FullEmoji), new Emoji(RollEmoji) };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;
            
            Console.WriteLine($"User {reaction.User} triggered {nameof(this.ProgressInteractiveReactions)} reaction {reaction.Emote.Name}");

            var message = userMessage.GetOrDownloadAsync().Result;
            if (!ProgressTracker.IsProgressTrackerMessage(message)) return Task.CompletedTask;

            if (reaction.Emote.Name == DecreaseEmoji)
            {
                DecreaseProgress(message);
                message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
            if (reaction.Emote.Name == IncreaseEmoji)
            {
                IncreaseProgress(message);
                message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
            if (reaction.Emote.Name == FullEmoji)
            {
                IncreaseProgressFullCheck(message);
                message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }
            if (reaction.Emote.Name == RollEmoji)
            {
                var tracker = new ProgressTracker(message);
                var roll = new ActionRoll(0, tracker.ActionDie, $"{ProgressResources.ProgressRollFor}{tracker.Title}");
                channel.SendMessageAsync(roll.ToString());
                message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            }

            return Task.CompletedTask;
        }

        [Command("ProgressTracker")]
        [Alias("Track", "Tracker", "Progress")]
        [Summary("Creates an objective tracking post for things like Iron Vows")]
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

            var msg = ReplyAsync(embed: embed.Build()).Result;

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