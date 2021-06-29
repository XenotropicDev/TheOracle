using Discord;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;

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

        public static string GetAnyPlanetLocation(string value)
        {
            if (value.Contains("Planetside", StringComparison.OrdinalIgnoreCase)) return "Planetside";
            if (value.Contains("Orbital", StringComparison.OrdinalIgnoreCase)) return "Orbital";
            if (value.Contains("Deep Space", StringComparison.OrdinalIgnoreCase)) return "Deep Space";
            if (value.Contains("Deepspace", StringComparison.OrdinalIgnoreCase)) return "Deep Space";
            return null;
        }

        public static SpaceRegion SpaceRegionFromEmote(string reactionName)
        {
            if (GenericReactions.oneEmoji.IsSameAs(reactionName)) return SpaceRegion.Terminus;
            if (GenericReactions.twoEmoji.IsSameAs(reactionName)) return SpaceRegion.Outlands;
            if (GenericReactions.threeEmoji.IsSameAs(reactionName)) return SpaceRegion.Expanse;
            return SpaceRegion.None;
        }

        /// <summary>
        /// Returns the value of any settlement locations contained in a string, and an empty string if not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="removeSettlement"></param>
        /// <returns></returns>
        public static string ExtractAnySettlementLocation(ref string value, bool removeSettlement = true)
        {
            string[] Locations = new string[] { "Planetside", "Orbital", "Deep space" };
            var temp = value;

            var SettlementLocation = Locations.FirstOrDefault(loc => temp.Contains(loc, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;

            if (SettlementLocation.Length > 0 && removeSettlement)
            {
                value = value.Replace(SettlementLocation, "", StringComparison.OrdinalIgnoreCase).Trim();
                value = Regex.Replace(value, "  +", " ");
            }

            return SettlementLocation;
        }
    }
}
