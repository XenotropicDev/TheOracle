using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public class OracleTable
    {
        public string[] Aliases { get; set; }
        public int d { get; set; } = 100;
        public string Category { get; set; }
        public string DisplayMode { get; set; }
        public bool DisplayChances { get; set; } = true;
        public GameName? Game { get; set; }
        public string Name { get; set; }
        public List<StandardOracle> Oracles { get; set; }
        public string Pair { get; set; } = string.Empty;
        public bool ShowResult { get; set; } = true;
        public OracleType Type { get; set; } = OracleType.standard;

        public bool MatchTableAlias(string table)
        {
            return this.Name.Equals(table, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => alias.Equals(table, StringComparison.OrdinalIgnoreCase)) == true;
        }

        internal bool ContainsTableAlias(string tableName, string[] additionalSearchTerms)
        {
            var temp1 = this.Name.Contains(tableName, StringComparison.OrdinalIgnoreCase) || this.Aliases?.Any(alias => alias.Contains(tableName, StringComparison.OrdinalIgnoreCase)) == true;
            var temp2 = additionalSearchTerms.Any(s => Name.Contains(s) || Aliases?.Contains(s) == true);
            return temp1 && temp2;
        }
    }
}