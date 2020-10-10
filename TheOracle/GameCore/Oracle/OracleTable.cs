using System.Collections.Generic;
using System.Dynamic;
using TheOracle.IronSworn;

namespace TheOracle.Core
{
    public class OracleTable
    {
        public string[] Aliases { get; set; }
        public int d { get; set; } = 100;
        public string DisplayMode { get; set; }
        public GameName? Game { get; set; }
        public string Name { get; set; }
        public List<StandardOracle> Oracles { get; set; }
        public string Pair { get; set; } = string.Empty;
        public bool ShowResult { get; set; } = true;
        public OracleType Type { get; set; } = OracleType.standard;
    }
}
