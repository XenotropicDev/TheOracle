using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public partial class OracleRoller
    {
        public OracleRoller(OracleService oracleService, GameName game, Random rnd = null)
        {
            OracleService = oracleService;
            Game = game;

            RollerRandom = rnd ?? BotRandom.Instance;
        }

        public OracleRoller(IServiceProvider services, GameName game, Random rnd = null)
        {
            OracleService = services.GetRequiredService<OracleService>();
            Game = game;

            RollerRandom = rnd ?? BotRandom.Instance;
        }

        public GameName Game { get; private set; }
        public OracleService OracleService { get; }
        public List<RollResult> RollResultList { get; set; } = new List<RollResult>();
        public IServiceProvider ServiceProvider { get; }
        private Random RollerRandom { get; set; }

        public OracleRoller BuildRollResults(string tableName)
        {
            RollResultList = new List<RollResult>();
            if (Game == GameName.None) Game = ParseOracleTables(tableName).FirstOrDefault()?.Game ?? GameName.None;

            RollFacade(tableName);

            return this;
        }

        public Embed GetEmbed()
        {
            string gameName = (Game != GameName.None) ? Game.ToString() + " " : string.Empty;

            EmbedBuilder embed = new EmbedBuilder().WithTitle($"__{gameName}{OracleResources.OracleResult}__");
            var footer = new EmbedFooterBuilder();
            foreach (var item in RollResultList)
            {
                embed.AddField($"{OracleResources.OracleTable} {item?.ParentTable?.Name} [{item.Roll}]", item.Result.Description, item.ShouldInline);

                if (item.ParentTable?.Pair?.Length > 0 && !RollResultList.Any(rr => rr.ParentTable.Name == item.ParentTable.Pair))
                {
                    footer.Text = (footer.Text == null || footer.Text.Length == 0) ? $"{OracleResources.PairedTable} {item.ParentTable.Pair}" : $"{CultureInfo.CurrentCulture.TextInfo.ListSeparator} {item.ParentTable.Pair}";
                    embed.WithFooter(footer);
                }
            }

            return embed.Build();
        }

        public List<OracleTable> ParseOracleTables(string tableName)
        {
            var result = new List<OracleTable>();

            // Match [table1/table2] style entries
            var match = Regex.Match(tableName, @"\[.*\]");
            if (match.Success)
            {
                var splits = tableName.Replace("[", "").Replace("]", "").Split('/');
                foreach (var item in splits)
                {
                    result.AddRange(OracleService.OracleList.Where(o => o.MatchTableAlias(item) && (Game == GameName.None || Game == o.Game)).ToList());
                }
            }
            else
            {
                result = OracleService.OracleList.Where(o => o.MatchTableAlias(tableName) && (Game == GameName.None || Game == o.Game)).ToList();
            }

            if (result.GroupBy(t => t.Game).Count() > 1)
            {
                string games = string.Empty;
                var gamesList = result.GroupBy(tbl => tbl.Game).Select(g => g.First());
                foreach (var g in gamesList) games += (g == gamesList.Last()) ? $"`{g.Game}`" : $"`{g.Game}`, ";
                throw new ArgumentException(string.Format(OracleResources.TooManyGamesError, games));
            }

            return result;
        }

        internal static OracleRoller RebuildRoller(OracleService oracleService, EmbedBuilder embed)
        {
            var roller = new OracleRoller(oracleService, Utilities.GetGameContainedInString(embed.Title));

            foreach (var field in embed.Fields)
            {
                var titleElementsRegex = Regex.Match(field.Name, OracleResources.OracleTable + @" ?(.*)\[(\d+)\]");
                var sourceTable = oracleService.OracleList.Find(oracle => oracle.Name == titleElementsRegex.Groups[1].Value.Trim());

                var oracleResult = oracleService.OracleList.Find(tbl => tbl.Name == sourceTable.Name)?.Oracles?.Find(oracle => oracle.Description == field.Value.ToString()) ?? null;

                if (!Int32.TryParse(titleElementsRegex.Groups[2].Value, out int tempRoll)) continue;

                roller.RollResultList.Add(new RollResult
                {
                    ParentTable = sourceTable,
                    Result = oracleResult,
                    ShouldInline = field.IsInline,
                    Roll = tempRoll
                });
            }

            return roller;
        }

        private void MultiRollFacade(string value, OracleTable multiRollTable, int depth)
        {
            int numberOfRolls;

            // Match [2x] style entries
            if (Regex.IsMatch(value, @"\[\d+x\]"))
            {
                var match = Regex.Match(value, @"\[(\d+)x\]");
                int.TryParse(match.Groups[1].Value, out numberOfRolls);
            }
            else
            {
                if (!int.TryParse(value, out numberOfRolls)) throw new ArgumentException($"Couldn't parse {value} as int");
            }

            for (int i = 1; i <= numberOfRolls; i++)
            {
                RollFacade(multiRollTable.Name, depth + 1);
            }
        }

        private void RollFacade(string table, int depth = 0)
        {
            table = table.Trim();

            var TablesToRoll = ParseOracleTables(table);

            if (TablesToRoll.Count == 0)
            {
                if (this.Game == GameName.None) throw new ArgumentException($"{OracleResources.UnknownTableError}{table}");

                //try again without any game name
                this.Game = GameName.None;
                RollFacade(table, depth);
            }

            foreach (var oracleTable in TablesToRoll)
            {
                int roll = RollerRandom.Next(1, oracleTable.d + 1);
                var oracleResult = oracleTable.Oracles.LookupOracle(roll);
                if (oracleTable.ShowResult)
                    RollResultList.Add(new RollResult
                    {
                        Roll = roll,
                        Result = oracleResult,
                        Depth = depth,
                        ShouldInline = oracleTable?.DisplayMode?.Equals("Inline", StringComparison.OrdinalIgnoreCase) ?? false,
                        ParentTable = oracleTable
                    });

                //Check if we have any nested oracles
                if (oracleResult.Oracles != null)
                {
                    RollNested(oracleResult, depth: 1);
                }

                //Check if we need to roll another oracle
                var match = Regex.Match(oracleResult.Description, @"^\[(.*)\]$");
                if (match.Success)
                {
                    string nextTable = match.Groups[0].Value;
                    if (Regex.IsMatch(nextTable, @"^\[\d+x\]"))
                    {
                        MultiRollFacade(nextTable, oracleTable, depth);
                        return;
                    }
                    RollFacade(nextTable, depth + 1);
                }

                // Match "{Place} of {Namesake}'s {Detail}" style entries
                var formatedStringMatches = Regex.Matches(oracleResult.Description, @"\{([^\}]*)\}").ToList();
                for (int i = formatedStringMatches.Count - 1; i >= 0; i--)
                {
                    Match formatMatch = formatedStringMatches[i];
                    var subTable = OracleService.OracleList.SingleOrDefault(o => o.MatchTableAlias(formatMatch.Groups[1].Value) && (Game == GameName.None || Game == o.Game));
                    if (subTable == null) continue;

                    var subRoller = new OracleRoller(OracleService, subTable.Game.GetValueOrDefault(), RollerRandom);
                    subRoller.BuildRollResults(subTable.Name);

                    var replacement = subRoller.RollResultList.Last().Result.Description;
                    string newDescription = oracleResult.Description.Substring(0, formatMatch.Index) + replacement + oracleResult.Description.Substring(formatMatch.Index + formatMatch.Length);
                    oracleResult.Description = newDescription;
                }
            }

            string output = string.Empty;
            foreach (var rollResult in RollResultList)
            {
                output += $"{OracleResources.Roll}: {rollResult.Roll} {OracleResources.Outcome}: {rollResult.Result.Description}\n";
            }
        }

        private void RollNested(StandardOracle oracleResult, int depth)
        {
            if (oracleResult == null || oracleResult.Oracles == null) return;

            //Todo fix it so the JSON can tell us what size die to roll
            int roll = RollerRandom.Next(1, 101);
            var innerRow = oracleResult.Oracles.LookupOracle(roll);

            if (innerRow == null) return;

            RollResultList.Add(new RollResult { Roll = roll, Result = innerRow, Depth = depth });

            if (innerRow.Oracles != null)
            {
                RollNested(innerRow, depth + 1);
            }
        }
    }
}