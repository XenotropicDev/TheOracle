using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.GameCore.Oracle
{
    public class RollResult
    {
        public int Depth { get; set; }
        public OracleTable ParentTable { get; set; }
        public StandardOracle Result { get; set; }
        public int Roll { get; set; }
        public bool ShouldInline { get; set; }
    }
}