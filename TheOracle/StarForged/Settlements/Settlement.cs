using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.StarForged
{
    public class Settlement
    {
        public Settlement(ServiceProvider services)
        {
            Services = services;
        }

        public string Authority { get; set; }
        public List<string> FirstLooks { get; set; }
        public int FirstLooksRevealed { get; set; }
        public string InitialContact { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Population { get; set; }
        public List<string> Projects { get; set; }
        public int ProjectsRevealed { get; set; }
        public int Seed { get; set; }
        public string SettlementTrouble { get; set; }
        public SpaceRegion Region { get; set; }
        public ServiceProvider Services { get; }

        public static Settlement GenerateSettlement(ServiceProvider serviceProvider, SpaceRegion spaceRegion, string SettlementName = "")
        {
            var oracleService = serviceProvider.GetRequiredService<OracleService>();
            if (SettlementName == string.Empty)
                SettlementName = oracleService.RandomRow("Settlement Name").Description;

            var s = new Settlement(serviceProvider);
            s.Seed = $"{SettlementName}{spaceRegion}".GetDeterministicHashCode();
            s.Region = spaceRegion;
            s.Name = SettlementName;

            Random random = new Random(s.Seed);

            s.Authority = oracleService.RandomRow("Settlement Authority", GameName.Starforged, random).Description;

            s.FirstLooks = oracleService.OracleList.Single(o => o.Name == "Settlement First Look" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.Description).ToList();
            s.FirstLooks.Shuffle(random);

            s.InitialContact = oracleService.RandomRow("Settlement Initial Contact", GameName.Starforged, random).Description;

            s.Location = oracleService.RandomRow("Settlement Location", GameName.Starforged, random).Description;

            s.Population = oracleService.RandomRow($"Settlement Population {s.Region}", GameName.Starforged, random).Description;

            s.Projects = oracleService.OracleList.Single(o => o.Name == "Settlement Projects" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.Description).ToList();
            s.Projects.Shuffle(random);

            s.SettlementTrouble = oracleService.RandomRow($"Settlement Trouble", GameName.Starforged, random).Description;

            return s;
        }

        public Settlement FromEmbed(Embed embed)
        {
            SpaceRegion region = StarforgedUtilites.GetAnySpaceRegion(embed.Fields.First(e => e.Name.Contains("Settlement Population")).Name);
            var settlement = GenerateSettlement(Services, region, embed.Title.Replace("__", ""));

            return settlement;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .AddField("Location", Location)
                .AddField("Population", Population, true)
                .AddField("Authority", Authority, true)

                .AddField("First Look", FirstLooks[0])
                .AddField("Initial Contact", InitialContact)
                .AddField("Settlement Projects", Projects[0])
                .AddField("Settlement Trouble", SettlementTrouble);

            return embedBuilder;
        }
    }
}