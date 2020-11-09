using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.Core;

namespace TheOracle.StarForged.Starships
{
    public class Starship
    {
        public List<string> FirstLooks { get; set; } = new List<string>();
        public int FirstLooksToReveal { get; set; } = 1;
        public string Fleet { get; set; }
        public string InitialContact { get; set; }
        public string Mission { get; set; }
        public string Name { get; set; }
        public string ShipType { get; set; }
        public SpaceRegion SpaceRegion { get; set; }
        public string TypicalRole { get; set; }
        public bool MissionRevealed { get; private set; }
        public IServiceProvider Services { get; }
        public ulong ChannelId { get; }

        public Starship(IServiceProvider services, ulong channelId)
        {
            Services = services;
            ChannelId = channelId;
        }

        public Starship FromEmbed(IEmbed embed)
        {
            string Name = embed.Title.Replace("__", "");
            SpaceRegion region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);

            this.FirstLooksToReveal = embed.Fields.Count(fld => fld.Name == StarShipResources.FirstLook);
            this.FirstLooks = embed.Fields.Where(fld => fld.Name == StarShipResources.FirstLook)?.Select(item => item.Value).ToList() ?? new List<string>();
            this.Fleet = embed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.Fleet).Value;
            this.InitialContact = embed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.InitialContact).Value;
            this.MissionRevealed = embed.Fields.Any(fld => fld.Name == StarShipResources.StarshipMission);
            if (this.MissionRevealed) this.Mission = embed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.StarshipMission).Value;
            this.Name = embed.Title.Replace("__", string.Empty);
            this.ShipType = embed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.StarshipType).Value;
            this.SpaceRegion = StarforgedUtilites.GetAnySpaceRegion(embed.Description);

            bool hasTypicalRole = embed.Fields.Any(fld => fld.Name == StarShipResources.TypicalRole);
            this.TypicalRole = (hasTypicalRole) ? embed.Fields.FirstOrDefault(fld => fld.Name == StarShipResources.TypicalRole).Value : string.Empty;

            return this;
        }

        public Starship AddMission()
        {
            var oracles = Services.GetRequiredService<OracleService>();
            this.Mission = oracles.RandomRow($"Starship Mission {SpaceRegion}", GameName.Starforged).Description;
            this.MissionRevealed = true;
            
            return this;
        }

        public static Starship GenerateShip(IServiceProvider services, SpaceRegion region, string name, ulong channelId)
        {
            if (region == SpaceRegion.None) throw new ArgumentException($"Please specify a space region");
            var oracles = services.GetRequiredService<OracleService>();

            Starship ship = new Starship(services, channelId);
            ship.Name = (name.Length > 0) ? name : oracles.RandomRow("Starship Name").Description;

            int seed = $"{ship.Name}{region}".GetDeterministicHashCode();

            Random random = new Random(seed);

            ship.FirstLooksToReveal = random.Next(1, 4);
            for (int i = 0; i < ship.FirstLooksToReveal; i++)
            {
                ship.FirstLooks.AddRandomOracleRow("Starship First Look", GameName.Starforged, services, channelId, random);
            }

            ship.Fleet = oracles.RandomRow($"Fleet", GameName.Starforged, random).Description;
            if (ship.Fleet.Equals(StarShipResources.StarshipMissionOracle)) ship.Fleet = oracles.RandomRow(string.Format(StarShipResources.StarshipMissionOracleRegionFormatter, region), GameName.Starforged).Description;

            ship.InitialContact = oracles.RandomRow("Starship Initial Contact", GameName.Starforged, random).Description;

            var shipTypeOracle = oracles.OracleList.Single(o => o.Name == "Starship Type" && o.Game == GameName.Starforged).Oracles.GetRandomRow(random);
            var shipType = (shipTypeOracle.Description != "[Starship Mission]") ? shipTypeOracle.Description : $"[Starship Mission {region}]";
            ship.ShipType = shipTypeOracle.GetOracleResult(services, GameName.Starforged, random);

            ship.TypicalRole = string.Empty;
            if (shipTypeOracle?.Prompt?.Length > 0)
            {
                ship.TypicalRole = shipTypeOracle.Prompt;
                if (ship.TypicalRole.Equals(StarShipResources.StarshipMissionOracle, StringComparison.OrdinalIgnoreCase))
                {
                    string additionalOracleTable = string.Format(StarShipResources.StarshipMissionOracleRegionFormatter, region);
                    ship.TypicalRole = oracles.RandomRow(additionalOracleTable, GameName.Starforged).Description;
                }
            }

            ship.SpaceRegion = region;

            return ship;
        }

        public EmbedBuilder GetEmbedBuilder()
        {
            var embed = new EmbedBuilder()
                .WithTitle($"__{Name}__")
                .WithDescription(String.Format(StarShipResources.StarshipDescription, SpaceRegion));

            embed.AddField(StarShipResources.StarshipType, ShipType, true);

            if (TypicalRole.Length > 0) embed.AddField(StarShipResources.TypicalRole, TypicalRole, true);

            embed.AddField(StarShipResources.Fleet, Fleet, true);
            embed.AddField(StarShipResources.InitialContact, InitialContact, true);

            for (int i = 0; i < FirstLooksToReveal; i++) embed.AddField(StarShipResources.FirstLook, FirstLooks[i], true);

            if (MissionRevealed) embed.AddField(StarShipResources.StarshipMission, Mission, true);

            return embed;
        }
    }
}