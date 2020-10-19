using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TheOracle.Core;
using TheOracle.IronSworn;

namespace TheOracle.StarForged.Settlements
{
    public class Settlement
    {
        public Settlement(ServiceProvider services)
        {
            Services = services;
        }

        public string Authority { get; set; }
        public int FirstLooksToReveal { get; set; }
        public List<string> FirstLooks { get; set; }
        public string InitialContact { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Population { get; set; }
        public List<string> Projects { get; set; }
        public int ProjectsRevealed { get; set; } = 0;
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
                .Oracles.Select(o => o.GetOracleResult(serviceProvider, GameName.Starforged, random)).ToList();
            s.FirstLooks.Shuffle(random);
            s.FirstLooksToReveal = random.Next(1, 3);

            s.InitialContact = oracleService.RandomRow("Settlement Initial Contact", GameName.Starforged, random).Description;

            s.Location = oracleService.RandomRow("Settlement Location", GameName.Starforged, random).Description;

            s.Population = oracleService.RandomRow($"Settlement Population {s.Region}", GameName.Starforged, random).Description;

            s.Projects = oracleService.OracleList.Single(o => o.Name == "Settlement Projects" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.GetOracleResult(serviceProvider, GameName.Starforged, random)).ToList();
            s.Projects.Shuffle(random);

            var trouble = oracleService.RandomRow($"Settlement Trouble", GameName.Starforged, random) as StandardOracle;
            s.SettlementTrouble = trouble.GetOracleResult(serviceProvider, GameName.Starforged, random);

            return s;
        }

        public Settlement FromEmbed(IEmbed embed)
        {
            if (!embed.Description.Contains(SettlementResources.Settlement)) throw new ArgumentException(SettlementResources.SettlementNotFoundError);

            SpaceRegion region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);
            var settlement = GenerateSettlement(Services, region, embed.Title.Replace("__", ""));

            settlement.FirstLooksToReveal = embed.Fields.Count(fld => fld.Name.Contains(SettlementResources.FirstLook));
            settlement.ProjectsRevealed = embed.Fields.Count(fld => fld.Name.Contains(SettlementResources.SettlementProjects));

            return settlement;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithDescription($"{Region} {SettlementResources.Settlement}")
                .AddField(SettlementResources.Location, Location, true)
                .AddField(SettlementResources.Population, Population, true)
                .AddField(SettlementResources.Authority, Authority, true);

            for (int i = 0; i < FirstLooksToReveal; i++) embedBuilder.AddField(SettlementResources.FirstLook, FirstLooks[i], true);

            embedBuilder.AddField(SettlementResources.InitialContact, $"||{InitialContact}||", true);
            embedBuilder.AddField(SettlementResources.SettlementTrouble, $"||{SettlementTrouble}||", true);

            for (int i = 0; i < ProjectsRevealed; i++) embedBuilder.AddField(SettlementResources.SettlementProjects, Projects[i], true);

            return embedBuilder;
        }
    }
}