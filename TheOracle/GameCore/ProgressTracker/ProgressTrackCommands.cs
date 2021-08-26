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

namespace TheOracle.GameCore.ProgressTracker
{
    public class ProgressTrackCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji DecreaseEmoji = new Emoji("\u25C0");

        public Emoji FullEmoji = new Emoji("\u0023\u20E3");
        public Emoji IncreaseEmoji = new Emoji("\u25B6");
        public Emoji oldFullEmoji = new Emoji("\u2714");
        public Emoji RollEmoji = new Emoji("\uD83C\uDFB2");
        public Emoji RecommitEmoji = new Emoji("❤️‍🩹");

        public ProgressTrackCommands(IServiceProvider service)
        {
            Service = service;
            Client = service.GetRequiredService<DiscordSocketClient>();
            var hooks = service.GetRequiredService<HookedEvents>();

            if (!hooks.ProgressReactions)
            {
                hooks.ProgressReactions = true;
                var reactionService = Service.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmote(GenericReactions.fiveEmoji).WithEvent(ProgressBuilderReactions).Build();

                ReactionEvent decrease = new ReactionEventBuilder().WithEmote(DecreaseEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent increase = new ReactionEventBuilder().WithEmote(IncreaseEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent fullMark2 = new ReactionEventBuilder().WithEmote(oldFullEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent fullMark = new ReactionEventBuilder().WithEmote(FullEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent roll = new ReactionEventBuilder().WithEmote(RollEmoji).WithEvent(ProgressInteractiveReactions).Build();
                ReactionEvent recommit = new ReactionEventBuilder().WithEmote(RecommitEmoji).WithEvent(ProgressInteractiveReactions).Build();

                reactionService.reactionList.Add(decrease);
                reactionService.reactionList.Add(increase);
                reactionService.reactionList.Add(fullMark);
                reactionService.reactionList.Add(fullMark2);
                reactionService.reactionList.Add(roll);
                reactionService.reactionList.Add(recommit);

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);
                reactionService.reactionList.Add(reaction5);
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
                if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) await BuildProgressTrackerPostAsync(ChallengeRank.Troublesome, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) await BuildProgressTrackerPostAsync(ChallengeRank.Dangerous, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) await BuildProgressTrackerPostAsync(ChallengeRank.Formidable, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) await BuildProgressTrackerPostAsync(ChallengeRank.Extreme, ThingToTrack, message).ConfigureAwait(false);
                if (reaction.Emote.IsSameAs(GenericReactions.fiveEmoji)) await BuildProgressTrackerPostAsync(ChallengeRank.Epic, ThingToTrack, message).ConfigureAwait(false);
            }).ConfigureAwait(false);

            return;
        }

        public async Task ProgressInteractiveReactions(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!ProgressTrackerInfo.IsProgressTrackerMessage(message)) return;

            if (reaction.Emote.IsSameAs(DecreaseEmoji))
            {
                DecreaseProgress(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.IsSameAs(IncreaseEmoji))
            {
                IncreaseProgress(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.IsSameAs(oldFullEmoji) || reaction.Emote.IsSameAs(FullEmoji))
            {
                IncreaseProgressFullCheck(message);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.IsSameAs(RollEmoji))
            {
                var tracker = new ProgressTrackerInfo().PopulateFromMessage(message);
                var roll = new ActionRoll(0, tracker.ActionDie, tracker.Description);
                await channel.SendMessageAsync(embed: roll.ToEmbed().WithAuthor($"Progress Roll").Build()).ConfigureAwait(false);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }
            if (reaction.Emote.IsSameAs(RecommitEmoji))
            {
                // "Roll both challenge dice, take the lowest value, and clear that number of progress boxes. Then, raise the vow’s rank by one (if not already epic).
                var roll = new ActionRoll();
                var amount = roll.ChallengeDie1 < roll.ChallengeDie2 ? roll.ChallengeDie1 : roll.ChallengeDie2;
                IncreaseRank(message);
                await message.ReplyAsync($"**Recommit:** rolled {roll.ChallengeDie1}, {roll.ChallengeDie2}; {amount} progress was removed, and its Challenge Rank was increased (if less than Epic).").ConfigureAwait(false);
                DecreaseProgressFullCheck(message, amount);
                await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            }

            return;
        }

        [Command("ProgressTracker")]
        [Alias("Track", "Tracker", "Progress")]
        [Summary("Creates an objective tracking post for things like Iron vows, journeys, and combat encounters")]
        [Remarks("\u25C0 - Decreases the progress track by the difficulty amount." +
            "\n\u25B6 - Increases the progress track by the difficulty amount." +
            "\n\u0023\u20E3 - Increases the progress track by a single full box (four ticks)." +
            "\n\uD83C\uDFB2 - Rolls the action and challenge die for the progress tracker." + "\n:mending_heart: - Recommits to a progress track after a miss (per Starforged), reducing progress by the lower of two challenge dice and increasing the challenge rank.")]
        public async Task ProgressTrackerCommand([Remainder] string TrackerArgs)
        {
            //TODO this all needs to be reworked for globalization
            string[] splitArgs = TrackerArgs.Split(' ');
            string NameOfTrack = TrackerArgs;
            ChallengeRank difficulty = ChallengeRank.None;

            if (Enum.TryParse(Enum.GetNames(typeof(ChallengeRank)).FirstOrDefault(cr => ProgressTrackerInfo.HasMatchingChallengeRank(cr, splitArgs)), out difficulty))
            {
                NameOfTrack = Regex.Replace(NameOfTrack, difficulty.ToString(), string.Empty, RegexOptions.IgnoreCase);
            }

            if (difficulty == ChallengeRank.None) await CreateTrackerHelper(NameOfTrack);
            else await BuildProgressTrackerPostAsync(difficulty, NameOfTrack);
        }

        private async Task BuildProgressTrackerPostAsync(ChallengeRank cr, string ThingToTrack, IUserMessage messageToEdit = null)
        {
            if (messageToEdit == null)
            {
                var tracker = new ProgressTrackerInfo(cr, ThingToTrack);
                messageToEdit = ReplyAsync(embed: tracker.BuildEmbed() as Embed).Result;
            }
            else
            {
                var tracker = new ProgressTrackerInfo().PopulateFromMessage(messageToEdit, cr);
                await messageToEdit.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = tracker.BuildEmbed() as Embed;
                });
            }

            await messageToEdit.RemoveAllReactionsAsync();

            _ = Task.Run(async () =>
            {
                await messageToEdit.AddReactionAsync(DecreaseEmoji);
                await messageToEdit.AddReactionAsync(IncreaseEmoji);
                await messageToEdit.AddReactionAsync(FullEmoji);
                await messageToEdit.AddReactionAsync(RollEmoji);
                await messageToEdit.AddReactionAsync(RecommitEmoji);
                await messageToEdit.AddReactionAsync(new Emoji(GenericReactions.recreatePostEmoji));
            }).ConfigureAwait(false);

            return;
        }

        private async Task CreateTrackerHelper(string nameOfTrack)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(ProgressResources.Progress_Tracker)
                .WithDescription(nameOfTrack)
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Reactions,
                    Value = $"{GenericReactions.oneEmoji} = {ProgressResources.Troublesome}" +
                            $"\n{GenericReactions.twoEmoji} = {ProgressResources.Dangerous}" +
                            $"\n{GenericReactions.threeEmoji} = {ProgressResources.Formidable}" +
                            $"\n{GenericReactions.fourEmoji} = {ProgressResources.Extreme}" +
                            $"\n{GenericReactions.fiveEmoji} = {ProgressResources.Epic}"
                });

            var msg = await ReplyAsync(embed: embed.Build());

            _ = Task.Run(async () =>
            {
                await msg.AddReactionAsync(GenericReactions.oneEmoji);
                await msg.AddReactionAsync(GenericReactions.twoEmoji);
                await msg.AddReactionAsync(GenericReactions.threeEmoji);
                await msg.AddReactionAsync(GenericReactions.fourEmoji);
                await msg.AddReactionAsync(GenericReactions.fiveEmoji);
            }).ConfigureAwait(false);

            return;
        }

        private void IncreaseRank(IUserMessage message)
        {
            ProgressTrackerInfo tracker = new ProgressTrackerInfo().PopulateFromMessage(message);
            ChallengeRank oldrank = tracker.Rank;
            if (oldrank == ChallengeRank.Troublesome)
            {
                tracker.Rank = ChallengeRank.Dangerous;
            }
            else if (oldrank == ChallengeRank.Dangerous)
            {
                tracker.Rank = ChallengeRank.Formidable;
            }
            else if (oldrank == ChallengeRank.Formidable)
            {
                tracker.Rank = ChallengeRank.Extreme;
            }
            else if (oldrank == ChallengeRank.Extreme)
            {
                tracker.Rank = ChallengeRank.Epic;
            }
            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed() as Embed);
        }

        private void DecreaseProgress(IUserMessage message)
        {
            ProgressTrackerInfo tracker = new ProgressTrackerInfo().PopulateFromMessage(message);

            tracker.RemoveProgress();

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed() as Embed).ConfigureAwait(false);
        }

        private void IncreaseProgress(IUserMessage message)
        {
            ProgressTrackerInfo tracker = new ProgressTrackerInfo().PopulateFromMessage(message);

            tracker.AddProgress();

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed() as Embed).ConfigureAwait(false);
        }

        private void IncreaseProgressFullCheck(IUserMessage message, int amount = 1)
        {
            ProgressTrackerInfo tracker = new ProgressTrackerInfo().PopulateFromMessage(message);

            tracker.Ticks += (4 * amount);

            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed() as Embed).ConfigureAwait(false);
        }

        private void DecreaseProgressFullCheck(IUserMessage message, int amount = 1)
        {
            ProgressTrackerInfo tracker = new ProgressTrackerInfo().PopulateFromMessage(message);
            tracker.Ticks -= (4 * amount);
            message.ModifyAsync(msg => msg.Embed = tracker.BuildEmbed() as Embed).ConfigureAwait(false);
        }

        private bool IsProgressTrackerMessage(IUserMessage message)
        {
            if (message.Embeds == null) return false;
            if (message.Embeds?.FirstOrDefault()?.Title == ProgressResources.Progress_Tracker) return true;

            return false;
        }
    }
}