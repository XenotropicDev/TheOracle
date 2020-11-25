using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;

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
        private IServiceProvider serviceProvider;
        private ulong channelId;

        public string BasicForm { get; set; }
        public CreatureEnvironment Environment { get; set; }
        public string EncounteredBehavior { get; set; }
        public List<string> FirstLook { get; set; } = new List<string>();
        public List<string> RevealedAspectsList { get; set; } = new List<string>();
        public string Scale { get; set; }
        public int Seed { get; set; }

        public Creature(IServiceProvider serviceProvider, ulong channelId)
        {
            this.serviceProvider = serviceProvider;
            this.channelId = channelId;
        }

        public static Creature GenerateNewCreature(IServiceProvider serviceProvider, ulong ChannelId, CreatureEnvironment environment, int seed = 0)
        {
            var creature = new Creature(serviceProvider, ChannelId);
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
                creature.FirstLook.AddRandomOracleRow("Creature First Look", GameName.Starforged, serviceProvider, ChannelId, rnd);
            }

            creature.Scale = oracles.RandomOracleResult($"Creature Scale", serviceProvider, GameName.Starforged, rnd);

            return creature;
        }

        public static Creature FromEmbed(IEmbed embed, IServiceProvider serviceProvider, ulong ChannelId)
        {
            var seedRegex = Regex.Match(embed.Footer.Value.Text, @"\d+");
            if (!seedRegex.Success || !int.TryParse(seedRegex.Value, out int seed)) throw new ArgumentException("Unknown creature seed value");

            var creature = new Creature(serviceProvider, ChannelId);
            creature.Environment = StarforgedUtilites.GetAnyEnvironment(embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.Environment).Value);
            creature.Scale = embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.Scale).Value;
            creature.BasicForm = embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.BasicForm).Value;
            creature.FirstLook = embed.Fields.Where(fld => fld.Name == CreatureResources.FirstLook).Select(fld => fld.Value).ToList();
            creature.EncounteredBehavior = embed.Fields.FirstOrDefault(fld => fld.Name == CreatureResources.EncounteredBehavior).Value;
            creature.RevealedAspectsList = embed.Fields.Where(fld => fld.Name == CreatureResources.RevealedAspect).Select(fld => fld.Value).ToList();

            return creature;
        }

        public Creature AddRandomAspect()
        {
            this.RevealedAspectsList.AddRandomOracleRow("Creature First Look", GameName.Starforged, serviceProvider, channelId);
            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            var builder = new EmbedBuilder().WithTitle(CreatureResources.CreatureTitle);

            builder.AddField(CreatureResources.Environment, Environment.ToString(), true);
            builder.AddField(CreatureResources.Scale, Scale, true);
            builder.AddField(CreatureResources.BasicForm, BasicForm, true);
            foreach (var s in FirstLook) builder.AddField(CreatureResources.FirstLook, s, true);
            builder.AddField(CreatureResources.EncounteredBehavior, EncounteredBehavior, true);
            for (int i = 0; i < RevealedAspectsList.Count; i++) builder.AddField(CreatureResources.RevealedAspect, RevealedAspectsList[i], true);
            builder.WithFooter(String.Format(CreatureResources.Seed, Seed));

            return builder;
        }
    }
}