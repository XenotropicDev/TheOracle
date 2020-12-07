using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;
using TheOracle.GameCore;
using TheOracle.GameCore.Oracle;

namespace TheOracle.StarForged.Settlements
{
    public class Settlement
    {
        public Settlement(IServiceProvider services, ulong channelId)
        {
            Services = services;
            ChannelId = channelId;
        }

        public string Authority { get; set; }
        public int FirstLooksToReveal { get; set; }
        public List<string> FirstLooks { get; set; } = new List<string>();
        public string InitialContact { get; set; }
        public bool InitialContactRevealed { get; private set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string Population { get; set; }
        public List<string> Projects { get; set; } = new List<string>();
        public int ProjectsRevealed { get; private set; } = 0;
        public string SettlementTrouble { get; set; }
        public bool SettlementTroubleRevealed { get; private set; }
        public SpaceRegion Region { get; set; }
        public IServiceProvider Services { get; }
        public ulong ChannelId { get; }

        public static Settlement GenerateSettlement(IServiceProvider serviceProvider, SpaceRegion spaceRegion, ulong channelId, string SettlementName = "", string SettlementLocation = "")
        {
            var oracleService = serviceProvider.GetRequiredService<OracleService>();
            if (SettlementName == string.Empty)
                SettlementName = oracleService.RandomRow("Settlement Name", GameName.Starforged).Description;

            var s = new Settlement(serviceProvider, channelId);
            s.Region = spaceRegion;
            s.Name = SettlementName;

            int seed = $"{SettlementName}{spaceRegion}".GetDeterministicHashCode();
            Random random = new Random(seed);

            s.Authority = oracleService.RandomRow("Settlement Authority", GameName.Starforged, random).Description;

            s.FirstLooksToReveal = random.Next(1, 4);
            for (int i = 0; i < s.FirstLooksToReveal; i++)
            {
                s.FirstLooks.AddRandomOracleRow("Settlement First Look", GameName.Starforged, serviceProvider, channelId, random);
            }

            s.Location = (SettlementLocation.Length > 0) ? SettlementLocation : oracleService.RandomRow("Settlement Location", GameName.Starforged, random).Description;

            s.Population = oracleService.RandomRow($"Settlement Population {s.Region}", GameName.Starforged, random).Description;

            return s;
        }

        public Settlement RevealTrouble()
        {
            var oracleService = Services.GetRequiredService<OracleService>();
            SettlementTrouble = oracleService.RandomOracleResult($"Settlement Trouble", Services, GameName.Starforged);
            SettlementTroubleRevealed = true;
            return this;
        }

        public Settlement RevealInitialContact()
        {
            var oracleService = Services.GetRequiredService<OracleService>();
            InitialContact = oracleService.RandomOracleResult($"Settlement Initial Contact", Services, GameName.Starforged);
            InitialContactRevealed = true;
            return this;
        }

        public Settlement AddProject()
        {
            Projects.AddRandomOracleRow("Settlement Projects", GameName.Starforged, Services, ChannelId);
            ProjectsRevealed++;
            return this;
        }

        public Settlement FromEmbed(IEmbed embed)
        {
            if (!embed.Description.Contains(SettlementResources.Settlement)) throw new ArgumentException(SettlementResources.SettlementNotFoundError);

            this.Authority = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Authority).Value;
            this.FirstLooks = embed.Fields.Where(fld => fld.Name == SettlementResources.FirstLook)?.Select(item => item.Value).ToList() ?? new List<string>();
            this.FirstLooksToReveal = FirstLooks.Count();
            this.InitialContactRevealed = embed.Fields.Any(fld => fld.Name == SettlementResources.InitialContact);
            if (InitialContactRevealed) this.InitialContact = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.InitialContact).Value;
            this.Location = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Location).Value;
            this.Name = embed.Title.Replace("__", "");
            this.Population = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.Population).Value;
            this.Projects = embed.Fields.Where(fld => fld.Name == SettlementResources.SettlementProjects)?.Select(item => item.Value).ToList() ?? new List<string>();
            this.ProjectsRevealed = embed.Fields.Count(fld => fld.Name.Contains(SettlementResources.SettlementProjects));
            this.Region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);
            this.SettlementTroubleRevealed = embed.Fields.Any(fld => fld.Name == SettlementResources.SettlementTrouble);
            if (SettlementTroubleRevealed) this.SettlementTrouble = embed.Fields.FirstOrDefault(fld => fld.Name == SettlementResources.SettlementTrouble).Value;

            return this;
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

            if (InitialContactRevealed) embedBuilder.AddField(SettlementResources.InitialContact, $"{InitialContact}", true);
            if (SettlementTroubleRevealed) embedBuilder.AddField(SettlementResources.SettlementTrouble, $"{SettlementTrouble}", true);

            for (int i = 0; i < ProjectsRevealed; i++) embedBuilder.AddField(SettlementResources.SettlementProjects, Projects[i], true);

            return embedBuilder;
        }
    }
}