using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.StarForged
{
    public class Planet
    {
        public int Seed { get; set; }
        public static Planet GeneratePlanet(string planetName)
        {
            var p = new Planet();

            p.Seed = planetName.GetDeterministicHashCode();
            Random PlanetRandom = new Random(p.Seed);

            PlanetTemplate Template = PlanetTemplate.GetPlanetTemplates().GetRandomRow(PlanetRandom);

            p.Atmosphere = Template.Atmospheres.GetRandomRow(PlanetRandom);
            p.Biomes = Biome.GetFromTemplate(Template, PlanetRandom);
            
            p.CloserLooks = Template.CloserLooks;
            p.CloserLooks.Shuffle(PlanetRandom);

            p.Description = Template.Description;
            p.Life = Template.PossibleLife.GetRandomRow(PlanetRandom);
            p.Name = planetName;
            p.PlanetType = Template.PlanetType;
            p.Settlements.Terminus = Template.PossibleSettlements.Terminus.GetRandomRow(PlanetRandom);
            p.Settlements.Outlands = Template.PossibleSettlements.Outlands.GetRandomRow(PlanetRandom);
            p.Settlements.Expanse = Template.PossibleSettlements.Expanse.GetRandomRow(PlanetRandom);
            p.Thumbnail = Template.Thumbnail;
            
            var so = new List<SpaceObservation>(Template.SpaceObservations);
            so.Shuffle(PlanetRandom);

            p.SpaceObservations = new List<SpaceObservation>(so.Take(PlanetRandom.Next(1,3)));

            return p;
        }

        public Planet()
        {
        }

        public string Name { get; set; }
        public string PlanetType { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public Atmosphere Atmosphere { get; set; } = new Atmosphere();
        public Settlements Settlements { get; set; } = new Settlements();
        public List<SpaceObservation> SpaceObservations { get; set; } = new List<SpaceObservation>();
        public int NumberOfBiomes { get => Biomes?.Count ?? 0; }
        public List<Biome> Biomes { get; set; } = new List<Biome>();
        public Life Life { get; set; }
        public List<CloserLook> CloserLooks { get; set; } = new List<CloserLook>();
    }
}