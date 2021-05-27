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

            if (info.Requires != null)
            {
                foreach (var prop in typeof(Requires).GetProperties())
                {
                    if (prop.GetValue(info.Requires) is IEnumerable<string> values)
                    {
                        foreach (var req in values)
                        {
                            tables.Add(new OracleTable()
                            {
                                OracleInfo = info,
                                Aliases = oracle.Aliases,
                                Parent = info.Name,
                                Category = info.Category,
                                Game = game,
                                Name = oracle.Name,
                                Oracles = ConverterHelpers.DataSwornTableToStandardOracle(oracle.Table),
                                Requires = req
                            });
                        }
                    }
                }
            }
            else
            {
                tables.Add(new OracleTable()
                {
                    OracleInfo = info,
                    Aliases = oracle.Aliases,
                    Parent = info.Name,
                    Category = info.Category,
                    Game = game,
                    Name = oracle.Name,
                    Oracles = ConverterHelpers.DataSwornTableToStandardOracle(oracle.Table)
                });
            }

            //Todo refactor this
            //This builds a oracle entry for each distinct table and requirement array value
            if (oracle.Tables != null)
            {
                foreach (var childTable in oracle.Tables)
                {
                    if (childTable.Requires == null)
                    {
                        tables.Add(new OracleTable()
                        {
                            OracleInfo = info,
                            Aliases = childTable.Aliases,
                            Category = info.Category,
                            Parent = info.Name,
                            Game = game,
                            Name = oracle.Name,
                            Requires = null,
                            Oracles = ConverterHelpers.DataSwornTableToStandardOracle(childTable.Table)
                        });

                        continue;
                    }

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
                                    Category = info.Category,
                                    Parent = info.Name,
                                    Game = game,
                                    Name = oracle.Name,
                                    Requires = req,
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