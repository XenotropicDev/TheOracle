using System;
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
                                DisplayName = oracle.DisplayName,
                                Parent = info.Name,
                                SearchTags = new List<string>() {info.Name, info.Parent },
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
                List<string> searchTags = new List<string>() { info.Name, info.Parent };
                searchTags.AddRange(info.Requires?.Life ?? Array.Empty<string>());
                searchTags.AddRange(info.Requires?.Location ?? Array.Empty<string>());

                tables.Add(new OracleTable()
                {
                    OracleInfo = info,
                    Aliases = oracle.Aliases,
                    DisplayName = oracle.DisplayName,
                    Parent = info.Name,
                    Category = info.Category,
                    Game = game,
                    Name = oracle.Name,
                    SearchTags = searchTags,
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
                        List<string> searchTags = new List<string>() { info.Name, info.Parent, oracle.Name };
                        searchTags.AddRange(childTable.Requires?.Life ?? Array.Empty<string>());
                        searchTags.AddRange(childTable.Requires?.Location ?? Array.Empty<string>());
                        
                        tables.Add(new OracleTable()
                        {
                            OracleInfo = info,
                            Aliases = childTable.Aliases,
                            DisplayName = childTable.DisplayName,
                            Category = info.Category,
                            Parent = info.Name,
                            Game = game,
                            Name = oracle.Name,
                            Requires = null,
                            SearchTags = searchTags,
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
                                List<string> searchTags = new List<string>() { info.Name, info.Parent, oracle.Name };
                                searchTags.AddRange(childTable.Requires?.Life ?? Array.Empty<string>());
                                searchTags.AddRange(childTable.Requires?.Location ?? Array.Empty<string>());

                                tables.Add(new OracleTable()
                                {
                                    OracleInfo = info,
                                    Aliases = childTable.Aliases,
                                    DisplayName = childTable.DisplayName,
                                    Category = info.Category,
                                    Parent = info.Name,
                                    Game = game,
                                    Name = oracle.Name,
                                    Requires = req,
                                    SearchTags = searchTags,
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