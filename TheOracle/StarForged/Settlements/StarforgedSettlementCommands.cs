using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.StarForged.Settlements
{
    public class StarforgedSettlementCommands : ModuleBase<SocketCommandContext>
    {
        public Emoji projectEmoji = new Emoji("\uD83D\uDEE0");
        public Emoji oneEmoji = new Emoji("\u0031\u20E3");
        public Emoji twoEmoji = new Emoji("\u0032\u20E3");
        public Emoji threeEmoji = new Emoji("\u0033\u20E3");

        public StarforgedSettlementCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarSettlementReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.ReactionAdded += SettlementReactionHandler;
                hooks.StarSettlementReactions = true;
            }

            Services = services;
        }

        private Task SettlementReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var emojisToProcess = new Emoji[] { projectEmoji, oneEmoji, twoEmoji, threeEmoji };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;

            var settlementHelperEmbed = message.Embeds.FirstOrDefault(embed => embed?.Title?.Contains(SettlementResources.SettlementHelper) ?? false);

            if (settlementHelperEmbed != null)
            {
                Console.WriteLine($"User {reaction.User} triggered {nameof(settlementHelperEmbed)} with reaction {reaction.Emote.Name}");

                var region = StarforgedUtilites.SpaceRegionFromEmote(reaction.Emote.Name);
                if (region == SpaceRegion.None) return Task.CompletedTask;

                string name = settlementHelperEmbed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.SettlementName).Value ?? string.Empty;

                var newSettlement = Settlement.GenerateSettlement(Services, region, name);
                Task.WaitAll(message.RemoveAllReactionsAsync());

                var task1 = message.ModifyAsync(msg =>
                {
                    msg.Content = string.Empty;
                    msg.Embed = newSettlement.GetEmbedBuilder().Build();
                });
                var task2 = message.AddReactionAsync(projectEmoji);
                return Task.WhenAll(task1, task2);
            }

            var settlmentEmbed = message.Embeds.FirstOrDefault(embed => embed?.Description?.Contains(SettlementResources.Settlement) ?? false);
            if (settlmentEmbed == null) return Task.CompletedTask;
            
            Console.WriteLine($"User {reaction.User} triggered {nameof(settlmentEmbed)} with reaction {reaction.Emote.Name}");

            var settlement = new Settlement(Services).FromEmbed(settlmentEmbed);

            if (reaction.Emote.Name == projectEmoji.Name) settlement.ProjectsRevealed++;

            message.ModifyAsync(msg => msg.Embed = settlement.GetEmbedBuilder().Build());
            message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);

            return Task.CompletedTask;
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