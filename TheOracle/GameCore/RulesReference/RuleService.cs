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
            if (File.Exists("IronSworn\\GameRules.json"))
            {
                var root = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("IronSworn\\GameRules.json"));
                Rules.AddRange(root.MovesReference);
            }

            if (File.Exists("Starforged\\GameRules.json"))
            {
                var root = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText("Starforged\\GameRules.json"));
                Rules.AddRange(root.MovesReference);
            }
        }
    }
}