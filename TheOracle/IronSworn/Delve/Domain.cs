using System;
using System.Collections.Generic;
using System.Text;
using TheOracle.GameCore;
using TheOracle.GameCore.RulesReference;

namespace TheOracle.IronSworn.Delve
{
    public class Domain
    {
        public string DelveSiteDomain { get; set; }
        public SourceInfo Source { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public List<Feature> Features { get; set; }
        public List<Danger> Dangers { get; set; }
    }
}
