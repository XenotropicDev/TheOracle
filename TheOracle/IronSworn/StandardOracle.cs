using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.IronSworn
{
    public class StandardOracle : IOracleEntry
    {
        public int d { get; set; } = 100;
        public OracleType type { get; set; } = OracleType.standard;
        public int Chance { get; set; }
        public string Description { get; set; }
    }
}
