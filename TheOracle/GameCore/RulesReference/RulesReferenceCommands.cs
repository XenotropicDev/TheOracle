using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using TheOracle.BotCore;
using TheOracle.GameCore.RulesReference;
using TheOracle.IronSworn;

namespace TheOracle.Core
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
                        await ReplyAsync($"No moves in {query}");
                        return;
                    }

                    string CategoryReply = string.Empty;
                    foreach (var move in rule.Moves)
                    {
                        CategoryReply += $"{move.Name}\n";
                    }

                    await ReplyAsync($"__**{rule.Category}**__:\n{CategoryReply}");
                }

                return;
            }

            string specificMovesReply = string.Empty;
            var specRules = ruleService.Rules.Where(r => r.Moves.Any(m => MatchNameOrAlias(m, query)));

            if (specRules.GroupBy(r => r.Game).Count() > 1) specRules = specRules.Where(r => r.Game == game);

            foreach (var rules in specRules)
            {
                var actualGame = rules.Game;
                foreach (var move in rules.Moves.Where(m => MatchNameOrAlias(m, query)))
                {
                    string temp = $"__{actualGame} - **{move.Name}**__\n{move.Text}\n\n";
                    if (specificMovesReply.Length + temp.Length > DiscordConfig.MaxMessageSize)
                    {
                        await ReplyAsync(specificMovesReply);
                        specificMovesReply = string.Empty;
                    }
                    specificMovesReply += temp;
                }
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