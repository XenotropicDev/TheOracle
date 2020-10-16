using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.StarForged
{
    public class StarforgedSettlementCommands : ModuleBase<SocketCommandContext>
    {
        public StarforgedSettlementCommands(ServiceProvider services)
        {
            var hooks = services.GetRequiredService<HookedEvents>();

            if (!hooks.StarSettlementReactions)
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.ReactionAdded += SettlementReactionHandler;
            }

            Services = services;
        }

        private Task SettlementReactionHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var emojisToProcess = new Emoji[] { null };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;

            //message.RemoveReactionsAsync(message.Author, emojisToProcess);
            //message.RemoveReactionsAsync(reaction.User.Value, emojisToProcess);

            return Task.CompletedTask;
        }

        public OracleService oracleService { get; set; }
        public ServiceProvider Services { get; }

        [Command("GenerateSettlement", ignoreExtraArgs: true)]
        [Summary("Creates a template post for a new Starforged settlement")]
        [Alias("Settlement")]
        public async Task SettlementPost(SpaceRegion region, [Remainder] string SettlementName = "")
        {
            var settlement = Settlement.GenerateSettlement(Services, region, SettlementName);

            //embedBuilder.ThumbnailUrl = planet.Thumbnail; //TODO (maybe location hex?)
            var message = await ReplyAsync("", false, settlement.GetEmbedBuilder().Build());
        }
    }
}