using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.GameCore.RulesReference;

namespace TheOracle.Core
{
    public class RulesReferenceCommands : ModuleBase<SocketCommandContext>
    {
        public RuleService ruleService { get; set; }

        [Command("QuickReference")]
        [Alias("Library", "QR", "Ref", "Reference")]
        [Summary("Creates an objective tracking post for things like Iron Vows")]
        public async Task ReferencePost([Remainder] string query)
        {
            query = query.Trim();
            if (ruleService.Rules.Any(r => MatchNameOrAlias(r, query)))
            {
                var rule = ruleService.Rules.Find(r => MatchNameOrAlias(r, query));
                
                if (rule.Moves.Count() == 0)
                {
                    await ReplyAsync($"No moves in {query}");
                    return;
                }

                string CategoryReply = string.Empty;
                foreach (var move in rule.Moves)
                {
                    //string aliases = string.Empty;
                    //if (move.Aliases != default)
                    //{
                    //    aliases = $" - Aliases: {string.Join(", ", move?.Aliases)}";
                    //}
                    CategoryReply += $"{move.Name}\n";
                }

                await ReplyAsync($"__**{rule.Category}**__:\n{CategoryReply}");
                return;
            }

            string specificMovesReply = string.Empty;
            foreach (var move in ruleService.Rules.SelectMany(r => r.Moves.Where(m => MatchNameOrAlias(m, query))))
            {
                specificMovesReply += $"__**{move.Name}**__\n{move.Text}\n\n";
            }

            if (specificMovesReply.Length == 0)
            {
                await ReplyAsync($"Unknown move: '{query}'");
                return;
            }

            await ReplyAsync(specificMovesReply);
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