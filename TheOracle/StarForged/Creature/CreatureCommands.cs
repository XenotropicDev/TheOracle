using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;

namespace TheOracle.StarForged.Settlements
{
    public class StarfrogedCreatureCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji projectEmoji = new Emoji("\uD83D\uDEE0");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");

        public StarfrogedCreatureCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarSettlementReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                var reactionService = services.GetRequiredService<ReactionService>();

                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(oneEmoji).WithEvent(SettlementReactionHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(twoEmoji).WithEvent(SettlementReactionHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(threeEmoji).WithEvent(SettlementReactionHandler).Build();

                ReactionEvent project = new ReactionEventBuilder().WithEmote(projectEmoji).WithEvent(SettlementReactionHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(project);

                hooks.StarSettlementReactions = true;
            }

            Services = services;
        }

        private async Task SettlementReactionHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            var settlementHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(SettlementResources.SettlementHelper) ?? false);

            if (settlementHelperEmbed != null)
            {
                var region = StarforgedUtilites.SpaceRegionFromEmote(reaction.Emote.Name);
                if (region == SpaceRegion.None) return;

                string name = settlementHelperEmbed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.SettlementName).Value ?? string.Empty;

                var newSettlement = Settlement.GenerateSettlement(Services, region, name);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                await message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newSettlement.GetEmbedBuilder().Build();
                }).ConfigureAwait(false);
                await message.AddReactionAsync(projectEmoji).ConfigureAwait(false);
                return;
            }

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return;

            var settlement = new Settlement(Services).FromEmbed(settlmentEmbed);

            if (reaction.Emote.Name == projectEmoji.Name) settlement.ProjectsRevealed++;

            await message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build()).ConfigureAwait(false);
            await message.RemoveReactionAsync(reaction.Emote, user).ConfigureAwait(false);

            return;
        }

        public OracleService oracleService { get; set; }
        public ServiceProvider Services { get; }

        [Command("GenerateSettlement", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged settlement")]
        [Alias("Settlement")]
        public async Task SettlementPost([Remainder] string SettlementCommand = "")
        {
            var region = StarforgedUtilites.GetAnySpaceRegion(SettlementCommand);

            if (region == SpaceRegion.None)
            {
                EmbedBuilder builder = new EmbedBuilder()
                    .WithTitle(SettlementResources.SettlementHelper)
                    .WithDescription(SettlementResources.PickSpaceRegionMessage);

                if (SettlementCommand.Length > 0) builder.WithFields(new EmbedFieldBuilder().WithName(SettlementResources.SettlementName).WithValue(SettlementCommand));

                var msg = await ReplyAsync(embed: builder.Build());
                await msg.AddReactionAsync(oneEmoji);
                await msg.AddReactionAsync(twoEmoji);
                await msg.AddReactionAsync(threeEmoji);
                return;
            }

            string SettlementName = SettlementCommand.Replace(region.ToString(), "").Trim();
            var settlement = Settlement.GenerateSettlement(Services, region, SettlementName);

            //embedBuilder.ThumbnailUrl = planet.Thumbnail; //TODO (maybe location hex?)
            var message = await ReplyAsync("", false, settlement.GetEmbedBuilder().Build());

            await message.AddReactionAsync(projectEmoji);
        }
    }
}