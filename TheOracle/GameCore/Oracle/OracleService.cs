using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TheOracle.IronSworn;

namespace TheOracle.Core
{
    public class OracleService
    {
        public List<OracleTable> OracleList { get; set; }

        public OracleService()
        {
            OracleList = new List<OracleTable>();
            
            //TODO change this to load more dynamically?
            var ironSworn = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("IronSworn\\oracles.json"));
            var starForged = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("StarForged\\StarforgedOracles.json"));
            OracleList.AddRange(ironSworn);
            OracleList.AddRange(starForged);
        }

        public IOracleEntry RandomRow(string TableName, GameName game = GameName.None, Random rand = null)
        {
            if (rand == null) rand = BotRandom.Instance;
            return OracleList.Single(ot => ot.Name == TableName && (ot.Game == game || game == GameName.None)).Oracles.GetRandomRow(rand);
        }
    }
}
