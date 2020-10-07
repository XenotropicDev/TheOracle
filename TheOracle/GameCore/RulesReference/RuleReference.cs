using System.Collections.Generic;

namespace TheOracle.GameCore.RulesReference
{
    public class RuleReference
    {
        public string Category { get; set; }
        public string[] Aliases { get; set; }
        public List<Move> Moves { get; set; }
    }

    public class Move
    {
        public string Name { get; set; }
        public string[] Aliases { get; set; }
        public string Text { get; set; }
    }
}