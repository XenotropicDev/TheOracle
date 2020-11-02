using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;

namespace TheOracle.StarForged.Creatures
{
    public enum CreatureEnvironment
    {
        None,
        Void,
        Interior,
        Land,
        Liquid,
        Air
    }

    public class Creature
    {
        private int revealedAspectsToShow = 0;

        public string BasicForm { get; set; }
        public CreatureEnvironment Environment { get; set; }
        public string EncounteredBehavior { get; set; }
        public List<string> FirstLook { get; set; } = new List<string>();
        public List<string> RevealedAspectsList { get; set; } = new List<string>();
        public int RevealedAspectsToShow { get => revealedAspectsToShow; set => revealedAspectsToShow = (value > 5) ? 5 : value; }
        public string Scale { get; set; }
        public int Seed { get; set; }

        public static Creature GenerateCreature(IServiceProvider serviceProvider, ulong ChannelId, CreatureEnvironment environment, int seed = 0)
        {
            var creature = new Creature();
            OracleService oracles = serviceProvider.GetRequiredService<OracleService>();
            Random rnd = new Random(seed);

            if (seed == 0)
            {
                rnd = BotRandom.Instance;
                seed = rnd.Next(1, int.MaxValue);
                rnd = new Random(seed);
            }
            creature.Seed = seed;

            if (environment == CreatureEnvironment.None) Enum.TryParse(oracles.RandomRow("Creature Environment", GameName.Starforged, rnd).Description, out environment);

            creature.BasicForm = oracles.RandomOracleResult($"Basic Form {environment}", serviceProvider, GameName.Starforged, rnd);
            creature.Environment = environment;
            creature.EncounteredBehavior = oracles.RandomOracleResult("Creature Encountered Behavior", serviceProvider, GameName.Starforged, rnd);

            int firstLookCount = rnd.Next(2, 4); //random.Next doesn't include the max value
            for (int i = 0; i < firstLookCount; i++)
            {
                creature.FirstLook.AddRandomOracleRow("Creature First Look", GameName.Starforged, ChannelId, serviceProvider, rnd);
            }

            for (int i = 0; i < 5; i++)
            {
                creature.RevealedAspectsList.AddRandomOracleRow("Creature Revealed Aspects", GameName.Starforged, ChannelId, serviceProvider, rnd);
            }

            creature.Scale = oracles.RandomOracleResult($"Creature Scale", serviceProvider, GameName.Starforged, rnd);

            return creature;
        }

        public static Creature FromEmbed(IEmbed embed, IServiceProvider serviceProvider, ulong ChannelId)
        {
            var seedRegex = Regex.Match(embed.Footer.Value.Text, @"\d+");
            if (!seedRegex.Success || !int.TryParse(seedRegex.Value, out int seed)) throw new ArgumentException("Unknown creature seed value");

            CreatureEnvironment environment = StarforgedUtilites.GetAnyEnvironment(embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.Environment).Value);

            var creature = GenerateCreature(serviceProvider, ChannelId, environment, seed);

            creature.RevealedAspectsToShow = embed.Fields.Count(fld => fld.Name == CreatureResources.RevealedAspect);
            return creature;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            var builder = new EmbedBuilder().WithTitle(CreatureResources.CreatureTitle);

            builder.AddField(CreatureResources.Environment, Environment.ToString(), true);
            builder.AddField(CreatureResources.Scale, Scale, true);
            builder.AddField(CreatureResources.BasicForm, BasicForm, true);
            foreach (var s in FirstLook) builder.AddField(CreatureResources.FirstLook, s, true);
            builder.AddField(CreatureResources.EncounteredBehavior, EncounteredBehavior, true);
            for (int i = 0; i < RevealedAspectsToShow; i++) builder.AddField(CreatureResources.RevealedAspect, RevealedAspectsList[i], true);
            builder.WithFooter(String.Format(CreatureResources.Seed, Seed));

            return builder;
        }
    }
}