using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace TheOracle.GameCore.RulesReference
{
    public class RuleService
    {
        private class Rootobject
        {
            public List<RuleReference> MovesReference { get; set; }
        }

        public List<RuleReference> Rules { get; set; } = new List<RuleReference>();

        public RuleService()
        {
            var ironRulesPath = Path.Combine("IronSworn","GameRules.json");
            var starRulesPath = Path.Combine("Starforged", "GameRules.json");
            if (File.Exists(ironRulesPath))
            {
                var root = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(ironRulesPath));
                Rules.AddRange(root.MovesReference);
            }

            if (File.Exists(starRulesPath))
            {
                var root = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(starRulesPath));
                Rules.AddRange(root.MovesReference);
            }
        }
    }
}