using System;

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
    }
}
