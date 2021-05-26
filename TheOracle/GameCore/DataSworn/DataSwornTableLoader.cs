using System.Collections.Generic;
using TheOracle.GameCore.DataSworn;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.Oracle
{
    public class DataSwornTableLoader
    {
        public List<OracleTable> GetTables(OracleInfo info, DataSworn.Oracle oracle, GameName game)
        {
            List<OracleTable> tables = new List<OracleTable>();

            var topTable = new OracleTable()
            {
                OracleInfo = info,
                Aliases = oracle.Aliases,
                Category = info.Category ?? info.Name,
                Game = game,
                Name = oracle.Name,
                Oracles = ConverterHelpers.DataSwornTableToStandardOracle(oracle.Table)
            };

            tables.Add(topTable);

            //Todo refactor this
            //This builds a oracle entry for each distinct table and requirement array value
            if (oracle.Tables != null)
            {
                foreach (var childTable in oracle.Tables)
                {
                    //Use reflection to get each property
                    foreach (var prop in typeof(Requires).GetProperties())
                    {
                        if (prop.GetValue(childTable.Requires) is IEnumerable<string> values)
                        {
                            foreach (var req in values)
                            {
                                tables.Add(new OracleTable()
                                {
                                    OracleInfo = info,
                                    Aliases = childTable.Aliases,
                                    Category = info.Category ?? info.Name,
                                    Game = game,
                                    Name = $"{oracle.Name} {req}",
                                    Oracles = ConverterHelpers.DataSwornTableToStandardOracle(childTable.Table)
                                });
                            }
                        }
                    }
                }
            }

            return tables;
        }
    }
}