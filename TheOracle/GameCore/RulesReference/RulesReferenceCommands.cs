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

            foreach (var rules in specRules)
            {
                var actualGame = rules.Game;
                foreach (var move in rules.Moves.Where(m => MatchNameOrAlias(m, query)))
                {
                    string sourceText = (move.Source != null) ? $"{move.Source}\n\n" : string.Empty;
                    string temp = $"__{actualGame} - **{move.Name}**__\n{move.Text}\n\n{sourceText}".Replace("\n\n\n", "\n\n");
                    if (specificMovesReply.Length + temp.Length > DiscordConfig.MaxMessageSize)
                    {
                        if (specificMovesReply.Length > 0)
                        {
                            await ReplyAsync(specificMovesReply);
                            specificMovesReply = string.Empty;
                        }

                        temp.Replace("\n\n\n", "\n\n");
                        if (temp.Length > DiscordConfig.MaxMessageSize)
                        {
                            int messageCutoff = temp.Substring(0, DiscordConfig.MaxMessageSize).LastIndexOf("\n");

                            var matches = Regex.Matches(temp, "```");
                            if (matches.Count > 0)
                            {
                                var match = matches.LastOrDefault(m => m.Index < DiscordConfig.MaxMessageSize && m.Index > temp.Length - DiscordConfig.MaxMessageSize);
                                if (match != default) messageCutoff = match.Index;
                            }
                            await ReplyAsync(temp.Substring(0, messageCutoff));
                            temp = temp.Substring(messageCutoff);
                        }
                    }
                    specificMovesReply += temp;
                }
            }

            if (specificMovesReply.Length == 0)
            {
                await ReplyAsync(string.Format(RulesResources.UnknownMoveError, query));
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