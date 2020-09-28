using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.IronSworn
{
    public class StandardOracle : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }
    }
}
