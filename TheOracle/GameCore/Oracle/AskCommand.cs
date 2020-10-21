using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.GameCore.Oracle
{
    public class AskCommand : ModuleBase<SocketCommandContext>
    {
        public const string oneEmoji = "\u0031\u20E3";
        public const string twoEmoji = "\u0032\u20E3";
        public const string threeEmoji = "\u0033\u20E3";
        public const string fourEmoji = "\u0034\u20E3";
        public const string fiveEmoji = "\u0035\u20E3";

        public AskCommand(ServiceProvider service)
        {
            Service = service;
            var hooks = service.GetRequiredService<HookedEvents>();

            if (!hooks.AskTheOracleReactions)
            {
                service.GetRequiredService<DiscordSocketClient>().ReactionAdded += HelperHandler;
                hooks.AskTheOracleReactions = true;
            }

            //Todo turn this into a localization friendly data structure
            ChanceLookUp = new List<Tuple<string, int>>
            {
                new Tuple<string, int>(OracleResources.AlmostCertain, 90),
                new Tuple<string, int>("Certain", 90),
                new Tuple<string, int>(OracleResources.Likely, 75 ),
                new Tuple<string, int>(OracleResources.FiftyFifty, 50 ),
                new Tuple<string, int>("5050", 50 ),
                new Tuple<string, int>(OracleResources.Unlikely, 25 ),
                new Tuple<string, int>(OracleResources.SmallChance, 10 ),
                new Tuple<string, int>("Small", 10 ),
            };
        }

        private Task HelperHandler(Cacheable<IUserMessage, ulong> userMessage, ISocketMessageChannel channel, SocketReaction reaction)
        {
            //TODO Concurrent queue so that users can't spam reactions?
            var emojisToProcess = new Emoji[] { new Emoji(oneEmoji), new Emoji(twoEmoji), new Emoji(threeEmoji), new Emoji(fourEmoji), new Emoji(fiveEmoji) };
            if (!reaction.User.IsSpecified || reaction.User.Value.IsBot || !emojisToProcess.Contains(reaction.Emote)) return Task.CompletedTask;

            var message = userMessage.GetOrDownloadAsync().Result;
            if (!message?.Embeds?.Any(embed => embed.Title == OracleResources.AskOracleHelperTitle) ?? false) return Task.CompletedTask;

            Task.Run(async () =>
            {
                await message.RemoveAllReactionsAsync();

                if (reaction.Emote.Name == oneEmoji) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(90, OracleResources.AlmostCertain); msg.Embed = null; });
                if (reaction.Emote.Name == twoEmoji) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(75, OracleResources.Likely); msg.Embed = null; });
                if (reaction.Emote.Name == threeEmoji) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(50, OracleResources.FiftyFifty); msg.Embed = null; });
                if (reaction.Emote.Name == fourEmoji) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(25, OracleResources.Unlikely); msg.Embed = null; });
                if (reaction.Emote.Name == fiveEmoji) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(10, OracleResources.SmallChance); msg.Embed = null; });
            });

            return Task.CompletedTask;
        }

        public List<Tuple<string, int>> ChanceLookUp { get; }
        public ServiceProvider Service { get; }

        [Command("AskTheOracle")]
        [Summary("Asks the oracle for the likelihood of something happening")]
        [Alias("Ask", "Chance")]
        public async Task AskTheOracleCommand([Remainder] string Likelihood = "")
        {
            Likelihood = Likelihood.Trim();
            if (Likelihood == string.Empty)
            {
                var helperEmbed = new EmbedBuilder().WithTitle(OracleResources.AskOracleHelperTitle).WithDescription(OracleResources.AskOracleHelperMessage);
                var msg = await ReplyAsync(embed: helperEmbed.Build());

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

            if (int.TryParse(Likelihood, out int chanceAsInt) && chanceAsInt > 0 && chanceAsInt < 100)
            {
                await ReplyAsync(AskTheOracleWithChance(chanceAsInt));
                return;
            }

            var lookupResult = ChanceLookUp.Find(chance => chance.Item1.Equals(Likelihood, StringComparison.OrdinalIgnoreCase));
            if (lookupResult != null)
            {
                await ReplyAsync(AskTheOracleWithChance(lookupResult.Item2, lookupResult.Item1));
                return;
            }

            string listOfOptions = String.Join(", ", ChanceLookUp.Select(lookup => $"`{lookup.Item2}`"));
            await ReplyAsync(OracleResources.AskCommandUnknownValue + listOfOptions);
        }

        private string AskTheOracleWithChance(int chance, string descriptor = "")
        {
            int roll = BotRandom.Instance.Next(1, 100);
            string result = (roll > chance) ? OracleResources.No : OracleResources.Yes;
            if (descriptor.Length > 0) descriptor = $"{descriptor.Trim()} ";
            return $"{OracleResources.AskResult} {descriptor}**{chance}** {OracleResources.Verus} {roll}\n**{result}**.";
        }
    }
}