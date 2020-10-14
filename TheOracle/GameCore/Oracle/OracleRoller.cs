using Discord;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public class OracleRoller
    {
        public class RollResult
        {
            public int Roll { get; set; }
            public StandardOracle Result { get; set; }
            public int Depth { get; set; }
            public bool ShouldInline { get; set; }
            public OracleTable ParentTable { get; set; }
        }

        internal static OracleRoller RebuildRoller(OracleService oracleService, EmbedBuilder embed)
        {
            var roller = new OracleRoller(oracleService);
            roller.RollResultList = new List<RollResult>();

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

        public OracleService OracleService { get; }
        public GameName Game { get; }
        public List<RollResult> RollResultList { get; set; }

        public OracleRoller(OracleService oracleService, GameName game = GameName.None)
        {
            OracleService = oracleService;
            Game = game;
        }

        public OracleRoller BuildRollResults(string tableName)
        {
            RollResultList = new List<RollResult>();
            RollFacade(tableName);

            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            string gameName = (Game != GameName.None) ? Game.ToString() + " " : string.Empty;

            EmbedBuilder embed = new EmbedBuilder().WithTitle($"__{gameName}{OracleResources.OracleResult}__");
            var footer = new EmbedFooterBuilder();
            foreach (var item in RollResultList)
            {
                embed.AddField($"{OracleResources.OracleTable} {item.ParentTable.Name} [{item.Roll}]", item.Result.Description, item.ShouldInline);

                if (item.ParentTable?.Pair?.Length > 0 && !RollResultList.Any(rr => rr.ParentTable.Name == item.ParentTable.Pair))
                {
                    footer.Text = (footer.Text == null || footer.Text.Length == 0) ? $"{OracleResources.PairedTable} {item.ParentTable.Pair}" : $"{CultureInfo.CurrentCulture.TextInfo.ListSeparator} {item.ParentTable.Pair}";
                    embed.WithFooter(footer);
                }
            }

            return embed;
        }

        private void RollFacade(string table, int depth = 0)
        {
            table = table.Trim();

            var TablesToRoll = ParseOracleTables(table);

            if (TablesToRoll.Count == 0)
            {
                throw new ArgumentException($"{OracleResources.UnknownTableError}{table}");
            }

            foreach (var oracleTable in TablesToRoll)
            {
                int roll = BotRandom.Instance.Next(1, oracleTable.d);
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
                    if (Regex.IsMatch(nextTable, @"^\[\d+x\]")) MultiRollFacade(nextTable, oracleTable, depth);
                    RollFacade(nextTable, depth + 1);
                }
            }

            string output = string.Empty;
            foreach (var rollResult in RollResultList)
            {
                output += $"{OracleResources.Roll}: {rollResult.Roll} {OracleResources.Outcome}: {rollResult.Result.Description}\n";
            }
        }

        private void RollNested(StandardOracle oracleResult, int depth, Random rnd = null)
        {
            if (oracleResult == null || oracleResult.Oracles == null) return;
            rnd ??= BotRandom.Instance;

            //Todo fix it so the JSON can tell us what size die to roll
            int roll = rnd.Next(1, 100);
            var innerRow = oracleResult.Oracles.LookupOracle(roll);

            if (innerRow == null) return;

            RollResultList.Add(new RollResult { Roll = roll, Result = innerRow, Depth = depth });

            if (innerRow.Oracles != null)
            {
                RollNested(innerRow, depth + 1, rnd);
            }
        }

        private List<OracleTable> ParseOracleTables(string tableName)
        {
            var result = new List<OracleTable>();

            // Match [table1/table2] style entries
            var match = Regex.Match(tableName, @"\[.*\]");
            if (match.Success)
            {
                var splits = tableName.Replace("[", "").Replace("]", "").Split('/');
                foreach (var item in splits)
                {
                    result.AddRange(OracleService.OracleList.Where(o => MatchTableAlias(o, item) && (Game == GameName.None || Game == o.Game)).ToList());
                }
            }
            else
            {
                result = OracleService.OracleList.Where(o => MatchTableAlias(o, tableName) && (Game == GameName.None || Game == o.Game)).ToList();
            }

            if (result.GroupBy(t => t.Game).Where(grp => grp.Count() > 1).Select(grp => grp.Key).Count() > 1)
            {
                string games = string.Empty;
                var gamesList = result.GroupBy(tbl => tbl.Game).Select(g => g.First());
                foreach (var g in gamesList) games += (g == gamesList.Last()) ? $"`{g.Game}`" : $"`{g.Game}`, ";
                throw new ArgumentException($"{OracleResources.TooManyGamesError}{games}");
            }

            return result;
        }

        private bool MatchTableAlias(OracleTable valueToCheck, string table)
        {
            return valueToCheck.Name.Equals(table, StringComparison.OrdinalIgnoreCase) || valueToCheck.Aliases?.Any(alias => alias.Equals(table, StringComparison.OrdinalIgnoreCase)) == true;
        }

        private string MultiRollFacade(string value, OracleTable multiRollTable, int depth)
        {
            int numberOfRolls;

            // Match [2x] style entries
            if (Regex.IsMatch(value, @"\[\d+x\]"))
            {
                var match = Regex.Match(value, @"\[(\d+)x\]");
                int.TryParse(match.Captures[0].Value, out numberOfRolls);
            }
            else
            {
                if (!int.TryParse(value, out numberOfRolls)) throw new ArgumentException($"Couldn't parse {value} as int");
            }

            string multiRollResult = string.Empty;
            for (int i = 1; i <= numberOfRolls; i++)
            {
                RollFacade(multiRollTable.Name, depth + 1);
            }

            return multiRollResult;
        }
    }
}