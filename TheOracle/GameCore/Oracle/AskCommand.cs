using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.Core;

namespace TheOracle.GameCore.Oracle
{
    public class AskCommand : ModuleBase<SocketCommandContext>
    {
        public AskCommand(ServiceProvider service)
        {
            Service = service;
            var hooks = service.GetRequiredService<HookedEvents>();

            if (!hooks.AskTheOracleReactions)
            {
                var reactionService = Service.GetRequiredService<ReactionService>();
                ReactionEvent reaction1 = new ReactionEventBuilder().WithEmote(GenericReactions.oneEmoji).WithEvent(HelperHandler).Build();
                ReactionEvent reaction2 = new ReactionEventBuilder().WithEmote(GenericReactions.twoEmoji).WithEvent(HelperHandler).Build();
                ReactionEvent reaction3 = new ReactionEventBuilder().WithEmote(GenericReactions.threeEmoji).WithEvent(HelperHandler).Build();
                ReactionEvent reaction4 = new ReactionEventBuilder().WithEmote(GenericReactions.fourEmoji).WithEvent(HelperHandler).Build();
                ReactionEvent reaction5 = new ReactionEventBuilder().WithEmote(GenericReactions.fiveEmoji).WithEvent(HelperHandler).Build();

                reactionService.reactionList.Add(reaction1);
                reactionService.reactionList.Add(reaction2);
                reactionService.reactionList.Add(reaction3);
                reactionService.reactionList.Add(reaction4);
                reactionService.reactionList.Add(reaction5);

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

        private async Task HelperHandler(IUserMessage message, ISocketMessageChannel channel, SocketReaction reaction, IUser user)
        {
            if (!message?.Embeds?.Any(embed => embed.Title == OracleResources.AskOracleHelperTitle) ?? false) return;

            await Task.Run(async () =>
            {
                await message.RemoveAllReactionsAsync();

                if (reaction.Emote.IsSameAs(GenericReactions.oneEmoji)) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(90, OracleResources.AlmostCertain); msg.Embed = null; });
                if (reaction.Emote.IsSameAs(GenericReactions.twoEmoji)) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(75, OracleResources.Likely); msg.Embed = null; });
                if (reaction.Emote.IsSameAs(GenericReactions.threeEmoji)) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(50, OracleResources.FiftyFifty); msg.Embed = null; });
                if (reaction.Emote.IsSameAs(GenericReactions.fourEmoji)) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(25, OracleResources.Unlikely); msg.Embed = null; });
                if (reaction.Emote.IsSameAs(GenericReactions.fiveEmoji)) await message.ModifyAsync(msg => { msg.Content = AskTheOracleWithChance(10, OracleResources.SmallChance); msg.Embed = null; });
            });

            return;
        }

        public List<Tuple<string, int>> ChanceLookUp { get; }
        public ServiceProvider Service { get; }

        [Command("AskTheOracle")]
        [Summary("Asks the oracle for the likelihood of something happening")]
        [Alias("Ask", "Chance")]
        public async Task AskTheOracleCommand([Remainder] string Likelihood = "")
        {
            Likelihood = Likelihood.Trim();

            var lookForDigits = Regex.Match(Likelihood, @"\d+");

            if (lookForDigits.Success && int.TryParse(lookForDigits.Value, out int chanceAsInt) && chanceAsInt > 0 && chanceAsInt < 100)
            {
                await ReplyAsync(AskTheOracleWithChance(chanceAsInt));
                return;
            }

            var lookupResult = ChanceLookUp.OrderByDescending(tup => tup.Item1.Length).ToList().Find(chance => Likelihood.Contains(chance.Item1, StringComparison.OrdinalIgnoreCase));
            if (lookupResult != null)
            {
                await ReplyAsync(AskTheOracleWithChance(lookupResult.Item2, lookupResult.Item1));
                return;
            }

            var helperEmbed = new EmbedBuilder().WithTitle(OracleResources.AskOracleHelperTitle).WithDescription(OracleResources.AskOracleHelperMessage);
            var msg = await ReplyAsync(embed: helperEmbed.Build());

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

        private string AskTheOracleWithChance(int chance, string descriptor = "")
        {
            int roll = BotRandom.Instance.Next(1, 101);
            string result = (roll > chance) ? OracleResources.No : OracleResources.Yes;
            if (descriptor.Length > 0) descriptor = $" ({descriptor.Trim()})";
            return $"{OracleResources.AskResult} {roll} {OracleResources.Verus} **{chance}**{descriptor}\n**{result}**.";
        }
    }
}