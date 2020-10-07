using System.Collections.Generic;
using System.Dynamic;
using TheOracle.IronSworn;

namespace TheOracle.Core
{
    public class OracleTable
    {
        public string Name { get; set; }
        public GameName? Game { get; set; }
        public int d { get; set; } = 100;
        public OracleType Type { get; set; } = OracleType.standard;
        public string Pair { get; set; } = string.Empty;
        public List<StandardOracle> Oracles { get; set; }
    }
}
