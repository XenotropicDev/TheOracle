using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TheOracle.Core;

namespace TheOracle.IronSworn
{
    public class OracleCommands : ModuleBase<SocketCommandContext>
    {
        //OracleService is loaded from DI
        public OracleCommands(OracleService oracleService)
        {
            _oracleService = oracleService;
        }

        private readonly OracleService _oracleService;

        [Command("OracleTable")]
        [Summary("Rolls an Oracle")]
        [Alias("Table")]
        public async Task OracleRollCommand(GameName gameNameUserVal = GameName.None, [Remainder] string TableName = "")
        {
            GameName? gameEnum = null;
            if (gameNameUserVal != GameName.None) gameEnum = gameNameUserVal;

            await ReplyAsync(RollOracleFacade(TableName, gameEnum));
        }

        [Command("OracleList", ignoreExtraArgs: false)]
        [Summary("Lists Availble Oracles")]
        [Alias("List")]
        public async Task OracleList()
        {
            EmbedBuilder builder = new EmbedBuilder();
            builder.Title = "Oracles";

            foreach (var oracle in _oracleService.OracleList)
            {
                string sample = string.Join(", ", oracle.Oracles.Take(3).Select(o => o.Description));
                builder.AddField(oracle.Name, $"sample: {sample}");
            }

            await ReplyAsync(string.Empty, false, builder.Build());
        }

        public string RollOracleFacade(string TableName, GameName? game = null)
        {
            var TablesToRoll = ParseOracleTables(TableName, game);

            if (TablesToRoll.Count == 0)
            {
                return $"Unknown Oracle Table: {TableName}";
            }

            string message = string.Empty;

            foreach (var oracleTable in TablesToRoll)
            {
                int roll = BotRandom.Instance.Next(1, oracleTable.d);
                var oracleResult = oracleTable.Oracles.LookupOracle(roll);

                //Check if we need to roll another oracle
                string extraOracleResult = string.Empty;
                var match = Regex.Match(oracleResult.Description, @"^\[(.*)\]$");
                if (match.Success)
                {
                    string nextTable = match.Groups[0].Value;
                    if (Regex.IsMatch(nextTable, @"^\[\d+x\]")) BuildMultiRoll(nextTable, oracleTable);
                    extraOracleResult += $" =>\n{RollOracleFacade(nextTable)}";
                }
                message += $"Checking the {TableName} table with a roll of {roll}:\n{oracleResult.Description}\n{extraOracleResult}";
            }

            return message;
        }

        private List<OracleTable> ParseOracleTables(string value, GameName? game = null)
        {
            // Match [table1/table2] style entries
            var match = Regex.Match(value, @"\[(([A-Za-z ]+)/)+([A-Za-z ]+)\]");
            if (match.Success)
            {
                var splits = value.Replace("[", "").Replace("]", "").Split('/');
                return _oracleService.OracleList.Where(o => splits.Contains(o.Name) && (game == null || game.Value == o.Game)).ToList();
            }

            var result = _oracleService.OracleList.Where(o => o.Name.Equals(value, StringComparison.OrdinalIgnoreCase) && (game == null || game.Value == o.Game)).ToList();
            if (result.Count > 1) throw new ArgumentException("Too many tables with that name, please specify a game");
            return result;
        }

        private string BuildMultiRoll(string value, OracleTable multiRollTable)
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
                multiRollResult += RollOracleFacade(multiRollTable.Name, multiRollTable.Game);
            }

            return multiRollResult;
        }
    }

    public enum GameName
    {
        None,
        Starforged,
        Ironsworn
    }
}