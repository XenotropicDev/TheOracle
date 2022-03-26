using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheOracle.GameCore.Oracle.DataSworn;

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
            var starRulesPath = Path.Combine("StarForged", "Data", "moves.json");
            if (File.Exists(ironRulesPath))
            {
                var root = JsonConvert.DeserializeObject<Rootobject>(File.ReadAllText(ironRulesPath));
                Rules.AddRange(root.MovesReference);
            }

            if (File.Exists(starRulesPath))
            {
                var root = JsonConvert.DeserializeObject<MovesInfo>(File.ReadAllText(starRulesPath));

                RuleReference currentCategoy = null;

                foreach (var move in root.Moves.OrderBy(m => m.Category))
                {
                    if (currentCategoy == null || move.Category != currentCategoy.Category)
                    {
                        if (currentCategoy != null) Rules.Add(currentCategoy); //Add the existing info before resting the object

                        currentCategoy = new RuleReference
                        {
                            Category = move.Category,
                            Source = new DataSworn.SourceAdapter(root.Source),
                            Game = GameName.Starforged,
                            Moves = new List<Move>()
                        };
                    }

                    currentCategoy.Moves.Add(new DataSworn.MoveAdapter(move, root.Source, GameName.Starforged));
                }

                Rules.Add(currentCategoy); //Add the last set to the service
            }
        }
    }
}