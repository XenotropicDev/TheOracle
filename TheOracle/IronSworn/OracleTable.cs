using System.Collections.Generic;
using TheOracle.IronSworn;

namespace TheOracle.Core
{
    public class OracleTable
    {
        public string Name { get; set; }
        public List<StandardOracle> Oracles { get; set; }
    }
}
