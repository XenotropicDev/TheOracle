using System.Collections.Generic;

namespace TheOracle.IronSworn
{
    public class StandardOracle : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }
        public string Prompt { get; set; }
        public List<StandardOracle> Oracles { get; set; }
    }
}