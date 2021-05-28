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

namespace TheOracle.GameCore.SceneChallenge
{
    public class SceneChallengeCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji DecreaseEmoji = new Emoji("\u25C0");
        public Emoji emptyChallengeEmoji = new Emoji("🟩");
        public Emoji fullChallengeEmoji = new Emoji("❎");
        public Emoji FullEmoji = new Emoji("\u0023\u20E3");
        public Emoji IncreaseEmoji = new Emoji("\u25B6");
        public Emoji RollEmoji = new Emoji("\uD83C\uDFB2");

        public SceneChallengeCommands(IServiceProvider services)
        {
            Services = services;

            var hooks = Services.GetRequiredService<HookedEvents>();

            if (!hooks.SceneChallengeReactions)
            {
                hooks.SceneChallengeReactions = true;
                var reactionService = Services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(ProgressBuilderReactions).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmote(GenericReactions.fiveEmoji).WithEvent(ProgressBuilderReactions).Build();

                ReactionEvent decrease = new ReactionEventBuilder().WithEmote(DecreaseEmoji).WithEvent(ReactionDecreaseProgressEvent).Build();
                ReactionEvent increase = new ReactionEventBuilder().WithEmote(IncreaseEmoji).WithEvent(ReactionIncreaseProgressEvent).Build();
                ReactionEvent fullMark = new ReactionEventBuilder().WithEmote(FullEmoji).WithEvent(ReactionFullMarkEvent).Build();
                ReactionEvent challengeDecrease = new ReactionEventBuilder().WithEmote(emptyChallengeEmoji).WithEvent(ReactionDecreaseChallengeEvent).Build();
                ReactionEvent challengeIncrease = new ReactionEventBuilder().WithEmote(fullChallengeEmoji).WithEvent(ReactionIncreaseChallengeEvent).Build();
                ReactionEvent roll = new ReactionEventBuilder().WithEmote(RollEmoji).WithEvent(ReactionResolveScene).Build();

                reactionService.reactionList.Add(decrease);
                reactionService.reactionList.Add(increase);
                reactionService.reactionList.Add(fullMark);
                reactionService.reactionList.Add(challengeDecrease);
                reactionService.reactionList.Add(challengeIncrease);
                reactionService.reactionList.Add(roll);

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);
                reactionService.reactionList.Add(reaction5);
            }
        }

        public IServiceProvider Services { get; }

        public async Task CreateTrackerHelper(string nameOfTrack)
        {
            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(SceneChallengeResources.SceneChallenge)
                .WithDescription(nameOfTrack)
                .WithFields(new EmbedFieldBuilder()
                {
                    Name = ProgressResources.Reactions,
                    Value = $"{GenericReactions.oneEmoji} = {ProgressResources.Troublesome}" +
                            $"\n{GenericReactions.twoEmoji} = {ProgressResources.Dangerous}" +
                            $"\n{GenericReactions.threeEmoji} = {ProgressResources.Formidable}" +
                            $"\n{GenericReactions.fourEmoji} = {ProgressResources.Extreme}" +
                            $"\n{GenericReactions.fiveEmoji} = {ProgressResources.Epic}"
                })
                .WithFooter(SceneChallengeResources.HelperAdditionalInfo);

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

        public async Task ProgressBuilderReactions(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

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

        [Command("SceneChallenge")]
        [Alias("Scene", "Challenge", "SceneTracker", "ChallengeTracker", "ChallengeScene")]
        [Summary("Creates a challenge scene tracking post.")]
        [Remarks("\u25C0 - Decreases the progress track by the difficulty amount." +
            "\n\u25B6 - Increases the progress track by the difficulty amount." +
            "\n\u0023\u20E3 - Increases the progress track by a single full box (four ticks)." +
            "\n🟩 - Decreases the challenge track by one. This is mostly needed to fix any accidental clicks." +
            "\n❎ - Increases the challenge track by one." +
            "\n\uD83C\uDFB2 - Rolls the action and challenge die to resolve the scene.")]
        public async Task SceneCommand([Remainder] string TrackerArgs)
        {
            //TODO this all needs to be reworked for globalization
            string[] splitArgs = TrackerArgs.Split(' ');
            string NameOfTrack = TrackerArgs;
            ChallengeRank difficulty = ChallengeRank.None;

            if (Enum.TryParse(Enum.GetNames(typeof(ChallengeRank)).FirstOrDefault(cr => ProgressTrackerInfo.HasMatchingChallengeRank(cr, splitArgs)), out difficulty))
            {
                NameOfTrack = Regex.Replace(NameOfTrack, difficulty.ToString(), string.Empty, RegexOptions.IgnoreCase);
            }

            if (difficulty == ChallengeRank.None) await CreateTrackerHelper(NameOfTrack).ConfigureAwait(false);
            else await BuildProgressTrackerPostAsync(difficulty, NameOfTrack).ConfigureAwait(false);
            return;
        }

        private async Task BuildProgressTrackerPostAsync(ChallengeRank cr, string nameOfTrack, IUserMessage messageToEdit = null)
        {
            if (messageToEdit == null)
            {
                var tracker = new SceneChallengeInfo(cr, nameOfTrack);
                messageToEdit = ReplyAsync(embed: tracker.BuildEmbed() as Embed).Result;
            }
            else
            {
                var tracker = new SceneChallengeInfo().FromMessage(messageToEdit, cr);
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
                await messageToEdit.AddReactionAsync(emptyChallengeEmoji);
                await messageToEdit.AddReactionAsync(fullChallengeEmoji);
                await messageToEdit.AddReactionAsync(RollEmoji);
                await messageToEdit.AddReactionAsync(new Emoji(GenericReactions.recreatePostEmoji));

            }).ConfigureAwait(false);

            return;
        }

        private bool IsSceneChallengeMessage(IUserMessage message)
        {
            return message.Embeds.FirstOrDefault()?.Title?.Equals(SceneChallengeResources.SceneChallenge, StringComparison.OrdinalIgnoreCase) ?? false;
        }

        private async Task ReactionDecreaseChallengeEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            scene.CountDownValue--;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = scene.BuildEmbed() as Embed);
        }

        private async Task ReactionDecreaseProgressEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            scene.Ticks -= scene.TicksPerProgress;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = scene.BuildEmbed() as Embed);
        }

        private async Task ReactionFullMarkEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            scene.Ticks += 4;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = scene.BuildEmbed() as Embed);
        }

        private async Task ReactionIncreaseChallengeEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            scene.CountDownValue++;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = scene.BuildEmbed() as Embed);
        }

        private async Task ReactionIncreaseProgressEvent(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;
            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            scene.Ticks += scene.TicksPerProgress;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ModifyAsync(msg => msg.Embed = scene.BuildEmbed() as Embed);
        }

        private async Task ReactionResolveScene(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsSceneChallengeMessage(message)) return;

            SceneChallengeInfo scene = new SceneChallengeInfo().FromMessage(message);
            var roll = new ActionRoll(0, scene.ActionDie, String.Format(SceneChallengeResources.ResolveSceneRoll, scene.Description));
            await channel.SendMessageAsync(roll.ToString()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }
    }
}