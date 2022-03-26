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

            List<string> searchTags = new List<string>() { info.Name };
            if (info.Aliases != null) searchTags.AddRange(info.Aliases);

            searchTags.AddRange(oracle.Requires?.Life ?? new List<string>());
            searchTags.AddRange(oracle.Requires?.Location ?? new List<string>());

            tables.Add(new OracleTable()
            {
                OracleInfo = info,
                Aliases = oracle.Aliases,
                DisplayName = oracle.DisplayName,
                Parent = info.Name,
                Category = oracle.Category,
                Game = game,
                Name = oracle.Name,
                SearchTags = searchTags,
                Oracles = ConverterHelpers.DataSwornTableToStandardOracle(oracle.Table)
            });

            //Todo refactor this
            //This builds a oracle entry for each distinct table and requirement array value
            if (oracle.Tables != null)
            {
                foreach (var childTable in oracle.Tables)
                {
                    if (childTable.Requires == null)
                    {
                        searchTags = new List<string>() { info.Name, oracle.Name };
                        searchTags.AddRange(childTable.Requires?.Life ?? new List<string>());
                        searchTags.AddRange(childTable.Requires?.Location ?? new List<string>());

                        tables.Add(new OracleTable()
                        {
                            OracleInfo = info,
                            Aliases = childTable.Aliases,
                            DisplayName = childTable.DisplayName,
                            Category = oracle.Category,
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
                                searchTags = new List<string>() { info.Name, oracle.Name };
                                searchTags.AddRange(childTable.Requires?.Life ?? new List<string>());
                                searchTags.AddRange(childTable.Requires?.Location ?? new List<string>());

                                tables.Add(new OracleTable()
                                {
                                    OracleInfo = info,
                                    Aliases = childTable.Aliases,
                                    DisplayName = childTable.DisplayName,
                                    Category = oracle.Category,
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