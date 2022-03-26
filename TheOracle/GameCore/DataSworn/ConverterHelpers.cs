using System.Collections.Generic;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.DataSworn
{
    public static class ConverterHelpers
    {
        internal static List<StandardOracle> DataSwornTableToStandardOracle(List<Oracle.DataSworn.Oracle> tables)
        {
            if (tables == null || tables.Count == 0) return null;

            var list = new List<StandardOracle>();

            foreach (var oracle in tables)
            {
                if (oracle.Table != null)
                {
                    foreach (var chanceTable in oracle.Table)
                    {
                        list.Add(new StandardOracle(chanceTable));
                    }
                }

                if (oracle.Tables != null)
                {
                    foreach (var subTableInfo in oracle.Tables)
                    {
                        foreach (var chanceTable in subTableInfo.Table)
                        {
                            list.Add(new StandardOracle(chanceTable));
                        }
                    }
                }
            }

            return list;
        }

        internal static List<StandardOracle> DataSwornTableToStandardOracle(List<ChanceTable> tables)
        {
            if (tables == null || tables.Count == 0) return null;

            var list = new List<StandardOracle>();

            foreach (var table in tables)
            {
                list.Add(new StandardOracle(table));
            }

            return list;
        }
    }
}