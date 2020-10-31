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
        public List<string> FirstLooks { get; set; }
        public int FirstLooksToReveal { get; set; }
        public string Fleet { get; set; }
        public string InitialContact { get; set; }
        public string Mission { get; set; }
        public string Name { get; set; }
        public int Seed { get; set; }
        public string ShipType { get; set; }
        public SpaceRegion SpaceRegion { get; set; }
        public string TypicalRole { get; set; }
        public bool MissionRevealed { get; set; } = false;

        public static Starship FromEmbed(ServiceProvider services, IEmbed embed)
        {
            string Name = embed.Title.Replace("__", "");
            SpaceRegion region = StarforgedUtilites.GetAnySpaceRegion(embed.Description);

            var ship = GenerateShip(services, region, Name);
            ship.MissionRevealed = embed.Fields.Any(fld => fld.Name.Equals(StarShipResources.StarshipMission));

            return ship;
        }

        public static Starship GenerateShip(ServiceProvider services, SpaceRegion region, string name)
        {
            if (region == SpaceRegion.None) throw new ArgumentException($"Please specify a space region");
            var oracles = services.GetRequiredService<OracleService>();

            Starship ship = new Starship();
            ship.Name = (name.Length > 0) ? name : oracles.RandomRow("Starship Name").Description;

            ship.Seed = $"{ship.Name}{region}".GetDeterministicHashCode();

            Random random = new Random(ship.Seed);

            ship.FirstLooks = oracles.OracleList.Single(o => o.Name == "Starship First Look" && o.Game == GameName.Starforged)
                .Oracles.Select(o => o.GetOracleResult(services, GameName.Starforged, random)).ToList();
            ship.FirstLooks.Shuffle(random);
            ship.FirstLooksToReveal = random.Next(1, 4);

            ship.Fleet = oracles.RandomRow($"Fleet", GameName.Starforged, random).Description;
            if (ship.Fleet.Equals(StarShipResources.StarshipMissionOracle)) ship.Fleet = oracles.RandomRow(string.Format(StarShipResources.StarshipMissionOracleRegionFormatter, region), GameName.Starforged).Description;

            ship.InitialContact = oracles.RandomRow("Starship Initial Contact", GameName.Starforged, random).Description;
            ship.Mission = oracles.RandomRow($"Starship Mission {region}", GameName.Starforged, random).Description;

            var shipType = oracles.OracleList.Single(o => o.Name == "Starship Type" && o.Game == GameName.Starforged).Oracles.GetRandomRow(random);
            ship.ShipType = shipType.GetOracleResult(services, GameName.Starforged, random);

            ship.TypicalRole = string.Empty;
            if (shipType?.Prompt?.Length > 0)
            {
                ship.TypicalRole = shipType.Prompt;
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

            for (int i = 0; i < FirstLooksToReveal - 1; i++) embed.AddField(StarShipResources.FirstLook, FirstLooks[i], true);

            if (MissionRevealed) embed.AddField(StarShipResources.StarshipMission, Mission, true);

            return embed;
        }
    }
}