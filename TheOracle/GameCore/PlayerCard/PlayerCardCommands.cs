using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOracle.BotCore;

namespace TheOracle.GameCore.PlayerCard
{
    public class PlayerCardCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji burnEmoji = new Emoji("🔥");
        public Emoji downEmoji = new Emoji("🔽");
        public Emoji fiveEmoji = new Emoji("\u0035\u20E3");
        public Emoji fourEmoji = new Emoji("\u0034\u20E3");
        public Emoji healthEmoji = new Emoji("❤️");
        public Emoji momentumEmoji = new Emoji("✈️");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji spiritEmoji = new Emoji("✨");
        public Emoji supplyEmoji = new Emoji("🎒");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji upEmoji = new Emoji("🔼");
        public PlayerCardCommands(IServiceProvider services)
        {
            Services = services;

            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.PlayerCardReactions)
            {
                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(oneEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(twoEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(threeEmoji).WithEvent(HelperReactionHandler).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(fourEmoji).WithEvent(HelperReactionHandler).Build();

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
                await message.AddReactionAsync(oneEmoji);
                await message.AddReactionAsync(twoEmoji);
                await message.AddReactionAsync(threeEmoji);
                await message.AddReactionAsync(fourEmoji);
            }).ConfigureAwait(false);
        }

        private async Task BurnMomentumReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (message.Embeds.FirstOrDefault()?.Title != PlayerResources.PlayerCardTitle) return;
            
            var player = new Player().PopulateFromEmbed(message.Embeds.First());
            player.Momentum = 2;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);
        }

        private async Task HelperReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var embed = message.Embeds.FirstOrDefault();
            if (embed.Title != PlayerResources.HelperTitle) return;

            int StatValue = 0;
            if (reaction.Emote.Name == oneEmoji.Name) StatValue = 1;
            if (reaction.Emote.Name == twoEmoji.Name) StatValue = 2;
            if (reaction.Emote.Name == threeEmoji.Name) StatValue = 3;
            if (reaction.Emote.Name == fourEmoji.Name) StatValue = 4;
            if (reaction.Emote.Name == fiveEmoji.Name) StatValue = 5;

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
            if (message.Embeds.FirstOrDefault()?.Title != PlayerResources.PlayerCardTitle) return;
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            if (!message.Reactions.TryGetValue(healthEmoji, out ReactionMetadata healthMetadata)) return;
            if (!message.Reactions.TryGetValue(healthEmoji, out ReactionMetadata supplyMetadata)) return;
            if (!message.Reactions.TryGetValue(healthEmoji, out ReactionMetadata spiritMetadata)) return;
            if (!message.Reactions.TryGetValue(healthEmoji, out ReactionMetadata momentumMetadata)) return;

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
            if (reaction.Emote.Name == upEmoji.Name) direction = 1;
            if (reaction.Emote.Name == downEmoji.Name) direction = -1;

            var player = new Player().PopulateFromEmbed(message.Embeds.First());
            if (healthActive.Result) player.Health = player.Health + direction;
            if (supplyActive.Result) player.Supply = player.Supply + direction;
            if (spiritActive.Result) player.Spirit = player.Spirit + direction;
            if (momentumActive.Result) player.Momentum = player.Momentum + direction;

            await message.ModifyAsync(msg => msg.Embed = player.GetEmbedBuilder().Build());
        }
    }
}
