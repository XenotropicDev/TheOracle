using Discord;
using Discord.Commands;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.RulesReference;

namespace TheOracle.GameCore.RulesReference
{
    public class RulesReferenceCommands : ModuleBase<SocketCommandContext>
    {
        public RuleService ruleService { get; set; }

        [Command("QuickReference")]
        [Alias("Library", "Ref", "Reference")]
        [Summary("Posts the rules text from the quick reference source document")]
        public async Task ReferencePost([Remainder] string query)
        {
            ChannelSettings channelSettings = await ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id);
            GameName game = Utilities.GetGameContainedInString(query);
            query = Utilities.RemoveGameNamesFromString(query);

            if (game == GameName.None && channelSettings != null) game = channelSettings.DefaultGame;

            query = query.Trim();

            if (ruleService.Rules.Any(r => MatchNameOrAlias(r, query)))
            {
                var rules = ruleService.Rules.Where(r => MatchNameOrAlias(r, query));

                if (rules.GroupBy(r => r.Game).Count() > 1) rules = rules.Where(r => r.Game == game);

                foreach (var rule in rules)
                {
                    if (rule.Moves.Count() == 0)
                    {
                        await ReplyAsync(string.Format(RulesResources.NoMovesError, query));
                        return;
                    }

                    string CategoryReply = string.Empty;
                    foreach (var move in rule.Moves)
                    {
                        CategoryReply += $"{move.Name}\n";
                    }

                    await ReplyAsync($"__**{rule.Category}**__:\n{CategoryReply}\n\n{rule.Source}");
                }

                return;
            }

            string specificMovesReply = string.Empty;
            var specRules = ruleService.Rules.Where(r => r.Moves.Any(m => MatchNameOrAlias(m, query)));

            if (specRules.GroupBy(r => r.Game).Count() > 1) specRules = specRules.Where(r => r.Game == game || game == GameName.None);

            int replies = 0;
            foreach (var rules in specRules)
            {
                var actualGame = rules.Game;
                foreach (var move in rules.Moves.Where(m => MatchNameOrAlias(m, query)))
                {
                    replies++;
                    foreach (var embed in move.AsEmbed(rules)) await ReplyAsync(embed: embed).ConfigureAwait(false);
                }
            }

            if (replies == 0)
            {
                await ReplyAsync(string.Format(RulesResources.UnknownMoveError, query));
                return;
            }
        }

        private bool MatchNameOrAlias(Move move, string name)
        {
            return move.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase) || move.Aliases?.Any(a => a.Equals(name, System.StringComparison.OrdinalIgnoreCase)) == true;
        }

        private bool MatchNameOrAlias(RuleReference rule, string name)
        {
            return rule.Category.Equals(name, System.StringComparison.OrdinalIgnoreCase) || rule.Aliases?.Any(a => a.Equals(name, System.StringComparison.OrdinalIgnoreCase)) == true;
        }
    }
}