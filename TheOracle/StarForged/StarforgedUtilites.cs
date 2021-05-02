using Discord;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;
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

        public static CreatureEnvironment GetAnyEnvironment(string value)
        {
            if (value.Contains("Space", StringComparison.OrdinalIgnoreCase) || value.Contains("Void", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Space;
            if (value.Contains("Interior", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Interior;
            if (value.Contains("Land", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Land;
            if (value.Contains("Liquid", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Liquid;
            if (value.Contains("Air", StringComparison.OrdinalIgnoreCase)) return CreatureEnvironment.Air;
            return CreatureEnvironment.None;
        }

        public static CreatureEnvironment CreatureEnvironmentFromEmote(string reactionName)
        {
            if (GenericReactions.oneEmoji.IsSameAs(reactionName)) return CreatureEnvironment.Space;
            if (GenericReactions.twoEmoji.IsSameAs(reactionName)) return CreatureEnvironment.Interior;
            if (GenericReactions.threeEmoji.IsSameAs(reactionName)) return CreatureEnvironment.Land;
            if (GenericReactions.fourEmoji.IsSameAs(reactionName)) return CreatureEnvironment.Liquid;
            if (GenericReactions.fiveEmoji.IsSameAs(reactionName)) return CreatureEnvironment.Air;
            return CreatureEnvironment.None;
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
