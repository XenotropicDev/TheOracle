using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheOracle.Core
{
    public class OracleService
    {
        public List<OracleTable> OracleList { get; set; }

        public OracleService()
        {
            OracleList = new List<OracleTable>();
            
            //TODO change this to load more dynamically?
            var oracleData = JsonConvert.DeserializeObject<List<OracleTable>>(File.ReadAllText("IronSworn\\oracles.json"));
            OracleList.AddRange(oracleData);
        }
    }
}
