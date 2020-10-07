using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public class OracleRoller
    {
        private class RollResult
        {
            public int Roll { get; set; }
            public string TableName { get; set; }
            public StandardOracle Result { get; set; }
            public int Depth { get; set; }
        }

        public OracleService OracleService { get; }
        public GameName Game { get; }
        private List<RollResult> RollResultList { get; set; }

        public OracleRoller(OracleService oracleService, GameName game = GameName.None)
        {
            OracleService = oracleService;
            Game = game;
        }

        public string RollTable(string tableName)
        {
            RollResultList = new List<RollResult>();
            
            RollFacade(tableName);

            string gameName = (Game != GameName.None) ? Game.ToString() + " " : string.Empty;

            string reply = string.Empty;
            foreach(var item in RollResultList)
            {
                string padding = new string('\t', item.Depth);
                if (item.TableName?.Length > 0) reply += $"{padding}Rolling the {gameName}oracle for {item.TableName}\n";
                reply += $"{padding}[{item.Roll}]: {item.Result.Description}\n";
            }
            return reply;
        }

        private void RollFacade(string table, int depth = 0)
        {
            table = table.Trim();
            var TablesToRoll = ParseOracleTables(table);

            if (TablesToRoll.Count == 0)
            {
                throw new ArgumentException($"Unknown Oracle Table: {table}");
            }

            foreach (var oracleTable in TablesToRoll)
            {
                int roll = BotRandom.Instance.Next(1, oracleTable.d);
                var oracleResult = oracleTable.Oracles.LookupOracle(roll);
                RollResultList.Add(new RollResult {Roll = roll, Result = oracleResult, TableName = oracleTable.Name, Depth = depth });

                
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
                output += $"Roll: {rollResult.Roll} Outcome: {rollResult.Result.Description}\n";
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

            RollResultList.Add(new RollResult {Roll = roll, Result = innerRow, Depth = depth });

            if (innerRow.Oracles != null)
            {
                RollNested(innerRow, depth + 1, rnd);
            }
        }

        private List<OracleTable> ParseOracleTables(string value)
        {
            var result = new List<OracleTable>();

            // Match [table1/table2] style entries
            var match = Regex.Match(value, @"\[.*\]");
            if (match.Success)
            {
                var splits = value.Replace("[", "").Replace("]", "").Split('/');
                result = OracleService.OracleList.Where(o => splits.Contains(o.Name) && (Game == GameName.None || Game == o.Game)).ToList();
            }
            else
            {
                result = OracleService.OracleList.Where(o => o.Name.Equals(value, StringComparison.OrdinalIgnoreCase) && (Game == GameName.None || Game == o.Game)).ToList();
            }

            if (result.Count > 1) throw new ArgumentException("Too many tables with that name, please specify a game");
            return result;
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
