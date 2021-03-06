using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.PlayerCard
{
    public class PlayerCardCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji burnEmoji = new Emoji("🔥");
        public Emoji downEmoji = new Emoji("🔽");
        public Emoji healthEmoji = new Emoji("❤️");
        public Emoji momentumEmoji = new Emoji("✈️");
        public Emoji spiritEmoji = new Emoji("✨");
        public Emoji supplyEmoji = new Emoji("🎒");
        public Emoji upEmoji = new Emoji("🔼");

        public PlayerCardCommands(IServiceProvider services)
        {
            Services = services;

            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.PlayerCardReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(HelperReactionHandler).Build();

                ReactionEvent upReaction = new ReactionEventBuilder().WithEmote(upEmoji).WithEvent(ResourceChangeHandler).Build();
                ReactionEvent downReaction = new ReactionEventBuilder().WithEmote(downEmoji).WithEvent(ResourceChangeHandler).Build();
                ReactionEvent burnReaction = new ReactionEventBuilder().WithEmote(burnEmoji).WithEvent(BurnMomentumReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);

                reactionService.reactionList.Add(upReaction);
                reactionService.reactionList.Add(downReaction);
                reactionService.reactionList.Add(burnReaction);

                hooks.PlayerCardReactions = true;
            }
        }

        public IServiceProvider Services { get; }

        [Summary("Creates a player stat tracking post\n• Use the reactions to set your active stat, then use 🔼 and 🔽 to change the value. Use 🔥 to burn/reset your momentum")]
        [Command("PlayerCard")]
        [Alias("StatsCard", "CharacterSheet", "CharSheet", "Player", "PC", "Character")]
        [Remarks("🔼 - Increase stat\n🔽 - Decrease stat\n❤️ - Set health as active stat\n✨ - Set spirit as active stat\n🎒 - Set supply as active stat\n✈️ - Set momentum as active stat")]
        public async Task CreatePlayerCard([Remainder] string CharacterName)
        {
            var helper = new EmbedBuilder()
                .WithTitle(PlayerResources.HelperTitle)
                .WithAuthor(CharacterName)
                .WithDescription("0,0,0,0,0");
            helper.AddField(PlayerResources.ActiveStat, PlayerResources.Edge);

            var message = await ReplyAsync(embed: helper.Build());

            _ = Task.Run(async () =>
            {
                await message.AddReactionAsync(GenericReactions.oneEmoji);
                await message.AddReactionAsync(GenericReactions.twoEmoji);
                await message.AddReactionAsync(GenericReactions.threeEmoji);
                await message.AddReactionAsync(GenericReactions.fourEmoji);
            }).ConfigureAwait(false);
        }

        [Summary("Uses inline replies to set the Debilities for a player card in the replied to message")]
        [Command("SetDebilities")]
        [Alias("SetDebility", "SetImpact", "SetImpacts")]
        [Remarks("Use an inline reply to set the number of debilities to a character card. The number of debilities are usually between 0 and 2")]
        public async Task SetDebilities(int numberOfDebilities)
        {
            if (!(Context.Message.ReferencedMessage is IUserMessage message) || !IsPlayerCardPost(message))
            {
                await ReplyAsync(PlayerResources.InlineReplyMissingError).ConfigureAwait(false);
                return;
            }

            var cs = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var player = new Player().WithChannelSettings(cs).PopulateFromEmbed(message.Embeds.First());
            player.Debilities = numberOfDebilities;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);
        }

        [Summary("Uses inline replies to set add XP for a player card in the replied to message.")]
        [Command("AddXP")]
        [Alias("XP")]
        [Remarks("Use a negative number to remove XP.")]
        public async Task AddXP(int amount)
        {
            if (!(Context.Message.ReferencedMessage is IUserMessage message) || !IsPlayerCardPost(message))
            {
                await ReplyAsync(PlayerResources.InlineReplyMissingError).ConfigureAwait(false);
                return;
            }

            var cs = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var player = new Player().WithChannelSettings(cs).PopulateFromEmbed(message.Embeds.First());
            player.UnspentXp += amount;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);
        }

        [Summary("Uses inline replies to mark XP as spent for a player card in the replied to message.")]
        [Command("SpendXP")]
        [Alias("Spend")]
        [Remarks("Use a negative value to undo/modify things. To directly set the spent and unspent XP use the `!EditField XP` command")]
        public async Task SpendXP(int amount)
        {
            if (!(Context.Message.ReferencedMessage is IUserMessage message) || !IsPlayerCardPost(message))
            {
                await ReplyAsync(PlayerResources.InlineReplyMissingError).ConfigureAwait(false);
                return;
            }

            var cs = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            var player = new Player().WithChannelSettings(cs).PopulateFromEmbed(message.Embeds.First());
            player.UnspentXp -= amount;
            player.SpentXp += amount;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);
        }

        private async Task BurnMomentumReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsPlayerCardPost(message)) return;

            var cs = await ChannelSettings.GetChannelSettingsAsync(channel.Id);
            var player = new Player().WithChannelSettings(cs).PopulateFromEmbed(message.Embeds.First());
            int startingMomentum = player.Momentum;
            player.Momentum = 2 - player.Debilities;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
            await message.ReplyAsync(String.Format(PlayerResources.BurnMomentumMessage, player.Name, startingMomentum));
        }

        private async Task HelperReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var embed = message.Embeds.FirstOrDefault();
            if (embed.Title != PlayerResources.HelperTitle) return;

            int StatValue = 0;
            if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) StatValue = 1;
            if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) StatValue = 2;
            if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) StatValue = 3;
            if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) StatValue = 4;
            if (reaction.Emote.IsSameAs(GenericReactions.fiveEmoji)) StatValue = 5;

            var stats = Array.ConvertAll(embed.Description.Split(','), int.Parse);

            string nextStat = string.Empty;
            var activeField = embed.Fields.First(embed => embed.Name == PlayerResources.ActiveStat);
            if (activeField.Value == PlayerResources.Edge) { stats[0] = StatValue; nextStat = PlayerResources.Heart; }
            if (activeField.Value == PlayerResources.Heart) { stats[1] = StatValue; nextStat = PlayerResources.Iron; }
            if (activeField.Value == PlayerResources.Iron) { stats[2] = StatValue; nextStat = PlayerResources.Shadow; }
            if (activeField.Value == PlayerResources.Shadow) { stats[3] = StatValue; nextStat = PlayerResources.Wits; }

            if (activeField.Value == PlayerResources.Wits)
            {
                stats[4] = StatValue;
                var player = new Player
                {
                    Edge = stats[0],
                    Heart = stats[1],
                    Iron = stats[2],
                    Shadow = stats[3],
                    Wits = stats[4],
                    Name = embed.Author.Value.Name,
                };

                await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);

                Emoji[] playerCardControlEmojis = new Emoji[] { upEmoji, downEmoji, healthEmoji, spiritEmoji, supplyEmoji, momentumEmoji, burnEmoji };

                await Task.Run(async () =>
                {
                    await message.RemoveAllReactionsAsync();
                    await message.AddReactionsAsync(playerCardControlEmojis);
                }).ConfigureAwait(false);

                return;
            }

            var newHelper = embed.ToEmbedBuilder().WithDescription(String.Join(',', stats));
            newHelper.Fields.RemoveAll(fld => fld.Name == activeField.Name);
            newHelper.AddField(PlayerResources.ActiveStat, nextStat);

            await message.ModifyAsync(msg => msg.Embed = newHelper.Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        private async Task ResourceChangeHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!IsPlayerCardPost(message)) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            var healthActive = message.GetReactionUsersAsync(healthEmoji, 5).AnyAsync(col => col.Any(u => u.Id == user.Id));
            var spiritActive = message.GetReactionUsersAsync(spiritEmoji, 5).AnyAsync(col => col.Any(u => u.Id == user.Id));
            var supplyActive = message.GetReactionUsersAsync(supplyEmoji, 5).AnyAsync(col => col.Any(u => u.Id == user.Id));
            var momentumActive = message.GetReactionUsersAsync(momentumEmoji, 5).AnyAsync(col => col.Any(u => u.Id == user.Id));

            Task[] tasks = new Task[] { healthActive.AsTask(), supplyActive.AsTask(), spiritActive.AsTask(), momentumActive.AsTask() };

            await Task.WhenAll(tasks);

            List<bool> activeResources = new List<bool> { healthActive.Result, supplyActive.Result, spiritActive.Result, momentumActive.Result };
            if (activeResources.Count(b => b) > 1)
            {
                var existingEmbed = message.Embeds.First().ToEmbedBuilder();
                existingEmbed.WithFooter(PlayerResources.TooManyStatsActive);

                await message.ModifyAsync(msg => msg.Embed = existingEmbed.Build()).ConfigureAwait(false);
                return;
            }

            int direction = 0;
            if (reaction.Emote.IsSameAs(upEmoji)) direction = 1;
            if (reaction.Emote.IsSameAs(downEmoji)) direction = -1;

            var cs = await ChannelSettings.GetChannelSettingsAsync(channel.Id);
            var player = new Player().WithChannelSettings(cs).PopulateFromEmbed(message.Embeds.First());
            if (healthActive.Result) player.Health += direction;
            if (supplyActive.Result) player.Supply += direction;
            if (spiritActive.Result) player.Spirit += direction;
            if (momentumActive.Result) player.Momentum += direction;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build());
        }

        private bool IsPlayerCardPost(IUserMessage message)
        {
            if (message.Embeds.FirstOrDefault()?.Author?.Name == PlayerResources.PlayerCardTitle) return true;
            if (message.Embeds.FirstOrDefault()?.Title == PlayerResources.PlayerCardTitle) return true; //Matching for old style cards.
            return false;
        }
    }
}