using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TheOracle.StarForged.Starships
{
    public class Starship
    {
        internal static Starship GenerateShip(ServiceProvider services, SpaceRegion region, string name)
        {
            throw new NotImplementedException();
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            throw new NotImplementedException();
        }

        public static Starship FromEmbed(ServiceProvider services, object settlmentEmbed)
        {
            throw new NotImplementedException();
        }
    }
}