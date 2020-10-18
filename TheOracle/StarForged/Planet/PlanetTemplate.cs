using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.StarForged
{
    public class PlanetTemplate : IOracleEntry
    {
        public string PlanetType { get; set; }
        public int Chance { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public List<Atmosphere> Atmospheres { get; set; } = new List<Atmosphere>();
        public PossibleSettlements PossibleSettlements { get; set; } = new PossibleSettlements();
        public int MaxSpaceObservations { get; set; }
        public List<SpaceObservation> SpaceObservations { get; set; } = new List<SpaceObservation>();
        public List<Numberofbiomes> NumberOfBiomes { get; set; } = new List<Numberofbiomes>();
        public List<Biome> PossibleBiomes { get; set; } = new List<Biome>();
        public List<Life> PossibleLife { get; set; } = new List<Life>();
        public List<CloserLook> CloserLooks { get; set; } = new List<CloserLook>();

        public static List<PlanetTemplate> GetPlanetTemplates()
        {
            var cache = BotCache.Get();
                
            var cachedPlanets = cache.GetOrCreate("PlanetTemplates", entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(10);
                string json = File.ReadAllText("StarForged\\Planet\\PlanetTemplates.json");
                return JsonConvert.DeserializeObject<List<PlanetTemplate>>(json);
            });

            return cachedPlanets;
        }
    }

    public class Atmosphere : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }

    public class PossibleSettlements
    {
        public List<Terminus> Terminus { get; set; } = new List<Terminus>();
        public List<Outlands> Outlands { get; set; } = new List<Outlands>();
        public List<Expanse> Expanse { get; set; } = new List<Expanse>();
    }

    public class SettlementRegions
    {
        public Terminus Terminus { get; set; }
        public Outlands Outlands { get; set; }
        public Expanse Expanse { get; set; }
    }
    

    public class Terminus : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }

    public class Outlands : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }

    public class Expanse : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }

    public class SpaceObservation : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }


    }

    public class Numberofbiomes
    {
        public int Chance { get; set; }
        public int Amount { get; set; }
    }

    public class Biome : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }


        internal static List<Biome> GetFromTemplate(PlanetTemplate template, Random planetRandom = null)
        {
            if (planetRandom == null) planetRandom = BotRandom.Instance;
            List<Biome> Biomes = new List<Biome>();
            int BiomesToGenerate = template.NumberOfBiomes.OrderBy(b => b.Chance).First(b => b.Chance >= planetRandom.Next(1, 100)).Amount;


            if (template.PossibleBiomes.Count <= 1 || BiomesToGenerate <= template.PossibleBiomes.Count)
            {
                Biomes.AddRange(template.PossibleBiomes);
                return Biomes;
            }
            
            if (BiomesToGenerate > 0)
            {
                for (int i = 1; i <= BiomesToGenerate; i++)
                {
                    Biome biome;
                    do
                    {
                        biome = template.PossibleBiomes.GetRandomRow(planetRandom);
                    } while (Biomes.Any(b => b.Chance == biome.Chance)); //don't add the same biomes more than once.

                    Biomes.Add(biome);
                }
            }
            return Biomes;
        }
    }

    public class Life : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }

    public class CloserLook : IOracleEntry
    {
        public int Chance { get; set; }
        public string Description { get; set; }

    }
}