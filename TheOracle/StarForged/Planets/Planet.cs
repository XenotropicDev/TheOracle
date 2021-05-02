using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Planets
{
    public class Planet
    {
        private IServiceProvider Services;

        public Planet(IServiceProvider service, ulong channelId)
        {
            Services = service;
            ChannelId = channelId;
        }

        public string Atmosphere { get; set; }
        public List<string> Biomes { get; set; } = new List<string>();
        public List<string> CloserLooks { get; set; } = new List<string>();
        public string Description { get; set; }
        public string Life { get; set; }
        public bool LifeRevealed { get; private set; }
        public string Name { get; set; }
        public int NumberOfBiomes { get => Biomes?.Count ?? 0; }
        public string PlanetType { get; set; }
        public int RevealedBiomes { get; private set; }
        public int RevealedLooks { get; private set; }
        public string Settlements { get; set; }
        public List<string> SpaceObservations { get; set; } = new List<string>();
        public SpaceRegion SpaceRegion { get; set; }
        public string Thumbnail { get; set; }
        public ulong ChannelId { get; }

        public static Planet GeneratePlanet(string planetName, SpaceRegion region, IServiceProvider services, ulong channelId, string planetType = "")
        {
            var p = new Planet(services, channelId);

            int seed = planetName.GetDeterministicHashCode();
            Random PlanetRandom = new Random(seed);

            PlanetTemplate Template = null;
            if (planetType?.Length == 0) Template = PlanetTemplate.GetPlanetTemplates().GetRandomRow(PlanetRandom);
            else Template = PlanetTemplate.GetPlanetTemplates().FirstOrDefault(t => t.PlanetType.Contains(planetType, StringComparison.OrdinalIgnoreCase));

            p.Atmosphere = Template.Atmospheres.GetRandomRow(PlanetRandom).Description;
            p.Biomes = Biome.GetFromTemplate(Template, PlanetRandom).Select(biome => biome.Description).ToList();

            p.Description = Template.Description;
            p.Life = Template.PossibleLife.GetRandomRow(PlanetRandom).GetOracleResult(services, GameName.Starforged);
            p.Name = planetName;
            p.PlanetType = Template.PlanetType;
            if (region == SpaceRegion.Terminus) p.Settlements = Template.PossibleSettlements.Terminus.GetRandomRow(PlanetRandom).GetOracleResult(services, GameName.Starforged);
            if (region == SpaceRegion.Outlands) p.Settlements = Template.PossibleSettlements.Outlands.GetRandomRow(PlanetRandom).GetOracleResult(services, GameName.Starforged);
            if (region == SpaceRegion.Expanse) p.Settlements = Template.PossibleSettlements.Expanse.GetRandomRow(PlanetRandom).GetOracleResult(services, GameName.Starforged);
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

        public static Planet GeneratePlanetFromEmbed(IEmbed embed, IServiceProvider services, ulong channelId)
        {
            var planet = new Planet(services, channelId);

            planet.Atmosphere = embed.Fields.FirstOrDefault(fld => fld.Name == PlanetResources.Atmosphere).Value;
            planet.CloserLooks = embed.Fields.Where(fld => fld.Name == PlanetResources.CloserLook)?.Select(item => item.Value).ToList() ?? new List<string>();
            planet.CloserLooks = embed.Fields.Where(fld => fld.Name == PlanetResources.Feature)?.Select(item => item.Value).ToList() ?? new List<string>();
            planet.Description = embed.Fields.FirstOrDefault(fld => fld.Name.Contains(PlanetResources.World)).Value;
            planet.LifeRevealed = embed.Fields.Any(fld => fld.Name == PlanetResources.Life);
            planet.Life = (planet.LifeRevealed) ? embed.Fields.FirstOrDefault(fld => fld.Name == PlanetResources.Life).Value : string.Empty;
            planet.Name = embed.Title.Replace("__", "");
            planet.PlanetType = embed.Fields.FirstOrDefault(fld => fld.Name.Contains(PlanetResources.World)).Name;
            planet.RevealedBiomes = embed.Fields.Count(field => field.Name == PlanetResources.Biome);
            planet.RevealedLooks = embed.Fields.Count(field => field.Name == PlanetResources.Feature) + embed.Fields.Count(field => field.Name == PlanetResources.CloserLook);
            planet.Settlements = embed.Fields.FirstOrDefault(fld => fld.Name == PlanetResources.Settlements).Value;
            planet.SpaceObservations = embed.Fields.Where(fld => fld.Name.Contains(String.Format(PlanetResources.SpaceObservation, string.Empty).Trim()))?.Select(item => item.Value).ToList() ?? new List<string>();
            planet.SpaceRegion = StarforgedUtilites.GetAnySpaceRegion(embed.Description);
            planet.Thumbnail = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : string.Empty;

            planet.Biomes = Planet.GeneratePlanet(planet.Name, planet.SpaceRegion, services, channelId, planet.PlanetType).Biomes; //Unfortunately we have to reuse the planet generator here so that we don't end up with a different number of biomes each roll

            return planet;
        }

        public Planet RevealLife()
        {
            this.Life = PlanetTemplate.GetPlanetTemplates().First(pt => pt.PlanetType == this.PlanetType).PossibleLife.GetRandomRow().GetOracleResult(Services, GameName.Starforged);
            this.LifeRevealed = true;
            return this;
        }

        public Planet RevealBiome()
        {
            this.RevealedBiomes++;
            return this;
        }

        public Planet RevealCloserLook()
        {
            this.CloserLooks.Add(PlanetTemplate.GetPlanetTemplates().First(pt => pt.PlanetType == this.PlanetType).CloserLooks.GetRandomRow().GetOracleResult(Services, GameName.Starforged));
            this.RevealedLooks++;
            return this;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder builder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithDescription(string.Format(PlanetResources.PlanetPostDescription, SpaceRegion))
                .WithThumbnailUrl(Thumbnail);

            builder.AddField(PlanetType, Description);
            builder.AddField(PlanetResources.Atmosphere, Atmosphere, true);
            builder.AddField(PlanetResources.Settlements, Settlements, true);

            for (int i = 0; i < SpaceObservations.Count; i++)
            {
                if (SpaceObservations.Count > 1) builder.AddField(string.Format(PlanetResources.SpaceObservation, i + 1), SpaceObservations[i], true);
                else builder.AddField(string.Format(PlanetResources.SpaceObservation, string.Empty), SpaceObservations[i], true);
            }

            if (LifeRevealed) builder.AddField(PlanetResources.Life, Life, true);
            
            for (int i = 0; i < RevealedLooks; i++) builder.AddField(PlanetResources.Feature, CloserLooks[i], true);
            
            for (int i = 0; i < RevealedBiomes; i++) builder.AddField(PlanetResources.Biome, Biomes[i], true);

            builder.Fields = builder.Fields.OrderBy(field => PlanetFieldOrder(field.Name)).ToList();

            return builder;
        }

        private int PlanetFieldOrder(string fieldName)
        {
            if (fieldName.Contains(PlanetResources.World)) return 1;
            if (fieldName.Contains(PlanetResources.Atmosphere)) return 2;
            if (fieldName == PlanetResources.Settlements) return 3;
            if (fieldName == PlanetResources.Life) return 7;
            if (fieldName.Contains(PlanetResources.Biome)) return 8;
            if (fieldName.Contains(string.Format(PlanetResources.SpaceObservation, string.Empty))) return 10;
            if (fieldName.Contains(string.Format(PlanetResources.SpaceObservation, 1))) return 11;
            if (fieldName.Contains(string.Format(PlanetResources.SpaceObservation, 2))) return 12;
            if (fieldName.Contains(string.Format(PlanetResources.SpaceObservation, 3))) return 13;
            if (fieldName.Contains(PlanetResources.Feature)) return 15;
            return 100;
        }
    }
}