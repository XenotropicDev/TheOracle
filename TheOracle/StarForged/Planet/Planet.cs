using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.StarForged
{
    public class Planet
    {
        private ServiceProvider Services;

        public Planet()
        {
        }

        public Atmosphere Atmosphere { get; set; } = new Atmosphere();
        public List<string> Biomes { get; set; } = new List<string>();
        public List<string> CloserLooks { get; set; } = new List<string>();
        public string Description { get; set; }
        public Life Life { get; set; }
        public bool LifeRevealed { get; set; } = false;
        public string Name { get; set; }
        public int NumberOfBiomes { get => Biomes?.Count ?? 0; }
        public string PlanetType { get; set; }
        public int RevealedBiomes { get; set; }
        public int RevealedLooks { get; set; } = 0;
        public int Seed { get; set; }
        public SettlementRegions Settlements { get; set; } = new SettlementRegions();
        public List<string> SpaceObservations { get; set; } = new List<string>();
        public SpaceRegion SpaceRegion { get; set; }
        public string Thumbnail { get; set; }

        public static Planet GeneratePlanet(string planetName, SpaceRegion region, ServiceProvider services)
        {
            var p = new Planet();
            p.Services = services;

            p.Seed = planetName.GetDeterministicHashCode();
            Random PlanetRandom = new Random(p.Seed);

            PlanetTemplate Template = PlanetTemplate.GetPlanetTemplates().GetRandomRow(PlanetRandom);

            p.Atmosphere = Template.Atmospheres.GetRandomRow(PlanetRandom);
            p.Biomes = Biome.GetFromTemplate(Template, PlanetRandom).Select(biome => biome.Description).ToList();

            p.Description = Template.Description;
            p.Life = Template.PossibleLife.GetRandomRow(PlanetRandom);
            p.Name = planetName;
            p.PlanetType = Template.PlanetType;
            p.Settlements.Terminus = Template.PossibleSettlements.Terminus.GetRandomRow(PlanetRandom);
            p.Settlements.Outlands = Template.PossibleSettlements.Outlands.GetRandomRow(PlanetRandom);
            p.Settlements.Expanse = Template.PossibleSettlements.Expanse.GetRandomRow(PlanetRandom);
            p.SpaceRegion = region;
            p.Thumbnail = Template.Thumbnail;

            var obsTemp = new List<string>();
            for (int i = 0; i < 5; i++)
            {
                p.CloserLooks.Add(Template.CloserLooks.GetRandomRow(PlanetRandom).GetOracleResult(p.Services, GameName.Starforged, PlanetRandom));
                obsTemp.Add(Template.SpaceObservations.GetRandomRow(PlanetRandom).GetOracleResult(p.Services, GameName.Starforged, PlanetRandom));
            }

            p.SpaceObservations = new List<string>(obsTemp.Take(PlanetRandom.Next(1, 3)));

            return p;
        }

        public static Planet GeneratePlanetFromEmbed(IEmbed embed, ServiceProvider services)
        {
            string name = embed.Title.Replace("__", "");
            SpaceRegion region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);
            var planet = GeneratePlanet(name, region, services);

            planet.RevealedLooks = embed.Fields.Count(field => field.Name == "Closer Look");
            planet.RevealedBiomes = embed.Fields.Count(field => field.Name == "Biome");
            planet.LifeRevealed = embed.Fields.Any(field => field.Name == "Life");

            return planet;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithDescription($"{SpaceRegion} Planet")
                .WithThumbnailUrl(Thumbnail);

            builder.AddField(PlanetType, Description);
            builder.AddField("Atmosphere", Atmosphere.Description, true);

            if (SpaceRegion == SpaceRegion.Terminus) builder.AddField("Settlements", Settlements.Terminus.Description, true);
            if (SpaceRegion == SpaceRegion.Outlands) builder.AddField("Settlements", Settlements.Outlands.Description, true);
            if (SpaceRegion == SpaceRegion.Expanse) builder.AddField("Settlements", Settlements.Expanse.Description, true);

            for (int i = 0; i < SpaceObservations.Count; i++)
            {
                if (SpaceObservations.Count > 1) builder.AddField($"Space Observation {i + 1}:", SpaceObservations[i], true);
                else builder.AddField($"Space Observation", SpaceObservations[i], true);
            }

            for (int i = 0; i < RevealedLooks; i++) builder.AddField("Closer Look", CloserLooks[i], true);
            for (int i = 0; i < RevealedBiomes; i++) builder.AddField("Biome", Biomes[i], true);

            if (LifeRevealed) builder.AddField("Life", Life.Description, true);

            builder.Fields = builder.Fields.OrderBy(field => PlanetFieldOrder(field.Name)).ToList();

            return builder;
        }

        private int PlanetFieldOrder(string fieldName)
        {
            if (fieldName.Contains("World")) return 1;
            if (fieldName.Contains("Atmosphere")) return 2;
            if (fieldName == "Settlements") return 3;
            if (fieldName == "Terminus Settlements") return 4;
            if (fieldName == "Outlands Settlements") return 5;
            if (fieldName == "Expanse Settlements") return 6;
            if (fieldName == "Life") return 9;
            if (fieldName.Contains("Observation")) return 10;
            if (fieldName.Contains("Observation 1")) return 11;
            if (fieldName.Contains("Observation 2")) return 12;
            if (fieldName.Contains("Observation 3")) return 13;
            if (fieldName.Contains("Closer Look")) return 15;
            return 100;
        }
    }
}