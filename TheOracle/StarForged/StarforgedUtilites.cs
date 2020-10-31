using System;
using TheOracle.StarForged.Creatures;

namespace TheOracle.StarForged
{
    public static class StarforgedUtilites
    {
        public static SpaceRegion GetAnySpaceRegion(string planetName)
        {
            if (planetName.Contains("Terminus", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Terminus;
            if (planetName.Contains("Outlands", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Outlands;
            if (planetName.Contains("Expanse", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Expanse;
            return SpaceRegion.None;
        }

        public static SpaceRegion SpaceRegionFromEmote(string reactionName)
        {
            if (reactionName == "\u0031\u20E3") return SpaceRegion.Terminus;
            if (reactionName == "\u0032\u20E3") return SpaceRegion.Outlands;
            if (reactionName == "\u0033\u20E3") return SpaceRegion.Expanse;
            return SpaceRegion.None;
        }

        public static CreatureEnvironment GetAnyEnvironment(string value)
        {
            if (value.Contains("Void", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Void;
            if (value.Contains("Interior", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Interior;
            if (value.Contains("Land", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Land;
            if (value.Contains("Liquid", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Liquid;
            if (value.Contains("Air", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Air;
            return CreatureEnvironment.None;
        }

        public static CreatureEnvironment CreatureEnvironmentFromEmote(string reactionName)
        {
            if (reactionName == "\u0031\u20E3") return CreatureEnvironment.Void;
            if (reactionName == "\u0032\u20E3") return CreatureEnvironment.Interior;
            if (reactionName == "\u0033\u20E3") return CreatureEnvironment.Land;
            if (reactionName == "\u0034\u20E3") return CreatureEnvironment.Liquid;
            if (reactionName == "\u0035\u20E3") return CreatureEnvironment.Air;
            return CreatureEnvironment.None;
        }
    }
}
