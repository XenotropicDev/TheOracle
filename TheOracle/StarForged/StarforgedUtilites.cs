using Discord;
using System;
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
    }
}
