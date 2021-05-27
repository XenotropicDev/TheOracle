using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.Oracle
{
    public partial class OracleTable
    {
        public OracleInfo OracleInfo { get; set; }
        public string[] Aliases { get; set; }
        public int d { get; set; } = 100;
        public string Category { get; set; }
        public string Parent { get; set; }
        public string DisplayMode { get; set; }
        public bool DisplayChances { get; set; } = true;
        public GameName? Game { get; set; }
        public string Name { get; set; }
        public List<StandardOracle> Oracles { get; set; }
        public string Pair { get; set; } = string.Empty;
        public bool ShowResult { get; set; } = true;
        public OracleType Type { get; set; } = OracleType.standard;
        public string Requires { get; internal set; }

        public bool MatchTableAlias(string table)
        {
            return this.Name.Equals(table, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => alias.Equals(table, StringComparison.OrdinalIgnoreCase)) == true;
        }

        internal bool ContainsTableAlias(string tableName, string[] additionalSearchTerms = null)
        {
            var temp1 = tableName.Contains(this.Name, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => tableName.Contains(alias, StringComparison.OrdinalIgnoreCase)) == true;
            var temp2 = (additionalSearchTerms == null) ? true : additionalSearchTerms.Any(s => Name.Contains(s, StringComparison.OrdinalIgnoreCase) || Aliases?.Contains(s) == true);
            return temp1 && temp2;
        }

        public bool MatchAll(string[] query)
        {
            foreach (var s in query)
            {
                if (Category?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) continue;
                if (Parent?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) continue;
                if (Name?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) continue;
                if (Game?.ToString().Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) continue;
                if (Requires?.ToString().Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) continue;
                if (Aliases?.Any(a => a.Contains(s, StringComparison.OrdinalIgnoreCase)) ?? false) continue;

                return false;
            }

            return true;
        }
    }
}