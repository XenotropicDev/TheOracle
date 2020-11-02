using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.GameCore.Oracle;

namespace TheOracle.Core
{
    public class OracleService
    {
        public List<OracleTable> OracleList { get; set; }

        public OracleService()
        {
            OracleList = new List<OracleTable>();

            foreach (var file in new DirectoryInfo("IronSworn\\").GetFiles("oracles.??.json"))
            {
            }

            if (File.Exists("IronSworn\\oracles.json"))
            {
                var ironSworn = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("IronSworn\\oracles.json"));
                OracleList.AddRange(ironSworn);
            }
            if (File.Exists("StarForged\\StarforgedOracles.json"))
            {
                var starForged = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("StarForged\\StarforgedOracles.json"));
                OracleList.AddRange(starForged);
            }
        }

        public IOracleEntry RandomRow(string TableName, GameName game = GameName.None, Random rand = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            try
            {
                return OracleList.Single(ot => ot.Name == TableName && (ot.Game == game || game == GameName.None)).Oracles.GetRandomRow(rand);
            }
            catch (Exception ex)
            {
                ArgumentException argEx = new ArgumentException($"Error retrieving oracle '{TableName}' for game '{game}'", ex);
                throw argEx;
            }
        }

        public string RandomOracleResult(string TableName, IServiceProvider serviceProvider, GameName game = GameName.None, Random rand = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            var row = RandomRow(TableName, game, rand);

            var tableData = OracleList.Single(ot => ot.Name == TableName && (ot.Game == game || game == GameName.None));
            game = tableData.Game ?? GameName.None;

            string lookup = row.Description;

            var match = Regex.Match(lookup, @"\[.*(\d+)x");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int rolls))
            {
                List<string> ReplaceMultiRollTables = new List<string>();
                for (int i = 0; i < rolls; i++)
                {
                    ReplaceMultiRollTables.Add(tableData.Name);
                }
                lookup = lookup.Replace($"{match.Groups[1]}x", string.Join("/", ReplaceMultiRollTables));
            }

            var roller = new OracleRoller(serviceProvider, game, rand);
            var tables = roller.ParseOracleTables(lookup);
            if (tables.Count == 0) return row.Description;
            roller.BuildRollResults(lookup);

            var finalResults = roller.RollResultList.Select(ocl => ocl.Result.Description);

            var spacer = (match.Success) ? " " : "\n";
            return $"{row.Description}{spacer}" + String.Join(" / ", finalResults);
        }
    }
}