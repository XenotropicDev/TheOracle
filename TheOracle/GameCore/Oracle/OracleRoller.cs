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
    public class OracleRoller
    {
        public OracleRoller(IServiceProvider serviceProvider, GameName game, Random rnd = null)
        {
            ServiceProvider = serviceProvider;
            OracleService = ServiceProvider.GetRequiredService<OracleService>();
            Game = game;

            RollerRandom = rnd ?? BotRandom.Instance;
        }

        public IServiceProvider ServiceProvider { get; }
        public GameName Game { get; private set; }
        public OracleService OracleService { get; }
        public List<RollResult> RollResultList { get; set; } = new List<RollResult>();
        private Random RollerRandom { get; set; }

        public OracleRoller BuildRollResults(string tableName)
        {
            if (Game == GameName.None) Game = ParseOracleTables(tableName).FirstOrDefault()?.Game ?? GameName.None;

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

        internal static OracleRoller RebuildRoller(OracleService oracleService, EmbedBuilder embed, IServiceProvider serviceProvider)
        {
            var roller = new OracleRoller(serviceProvider, Utilities.GetGameContainedInString(embed.Title));

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
                var Context = ServiceProvider.GetService<CommandContext>();
                ChannelSettings channelSettings = ChannelSettings.GetChannelSettingsAsync(Context.Channel.Id).Result;

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

        public class RollResult
        {
            public int Depth { get; set; }
            public OracleTable ParentTable { get; set; }
            public StandardOracle Result { get; set; }
            public int Roll { get; set; }
            public bool ShouldInline { get; set; }
        }
    }
}