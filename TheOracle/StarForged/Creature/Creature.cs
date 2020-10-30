using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TheOracle.Core;

namespace TheOracle.StarForged.Creature
{
    public class Creature
    {
        public CreatureEnvironment creatureEnvironment { get; set; }
        public string Scale { get; set; }
        public string BasicForm { get; set; }
        public List<string> FirstLook { get; set; }
        public string EncounteredBehavior { get; set; }
        public List<string> RevealedAspect { get; set; }
        public int Seed { get; set; }

        public EmbedBuilder GetEmbedBuilder()
        {
            var builder = new EmbedBuilder()
                .WithTitle("")
                .WithDescription("");

            builder.AddField(CreatureResources.Environment, creatureEnvironment.ToString(), true);
            builder.AddField(CreatureResources.Scale, Scale, true);
            builder.AddField(CreatureResources.BasicForm, BasicForm, true);
            foreach (var s in FirstLook) builder.AddField(CreatureResources.FirstLook, s, true);
            builder.AddField(CreatureResources.EncounteredBehavior, EncounteredBehavior, true);
            foreach (var s in RevealedAspect) builder.AddField(CreatureResources.RevealedAspect, s, true);

            return builder;
        }

        public static Creature GenerateCreature(IServiceProvider serviceProvider, CreatureEnvironment environment, int seed = 0)
        {
            var creature = new Creature();
            OracleService oracles = serviceProvider.GetRequiredService<OracleService>();
            Random rnd = new Random(seed);

            if (seed == 0)
            {
                rnd = new Random();
                seed = rnd.Next(1, int.MaxValue);
                rnd = new Random(seed);
            }

            if (environment == CreatureEnvironment.None) Enum.TryParse(oracles.RandomRow("Creature Environment").Description, out environment);

            return creature;
        }
    }

    public enum CreatureEnvironment
    {
        None,
        Void,
        Interior,
        Land,
        Liquid,
        Air
    }
}