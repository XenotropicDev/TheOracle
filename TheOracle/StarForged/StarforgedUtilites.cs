using System;

namespace TheOracle.StarForged
{
    public static class StarforgedUtilites
    {
        public static SpaceRegion GetAnySpaceRegion(string planetName)
        {
            if (planetName.Contains("Terminus", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Terminus;
            if (planetName.Contains("Outlands", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Outloads;
            if (planetName.Contains("Expanse", StringComparison.OrdinalIgnoreCase)) return SpaceRegion.Expanse;
            return SpaceRegion.None;
        }
    }
}
