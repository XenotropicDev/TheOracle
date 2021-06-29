using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.GameCore.Oracle;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.DataSworn
{
    public static class ConverterHelpers
    {
        internal static List<object> DataSwornTableToStandardOracle(List<Table> tables)
        {
            if (tables == null || tables.Count == 0) return null;

            var list = new List<object>();

            foreach (var table in tables) list.Add(table);

            return list;
        }
    }
}
