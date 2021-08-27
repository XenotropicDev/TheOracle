using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.GameCore;
using TheOracle.GameCore.Assets;

namespace TheOracle.StarForged.PlayerShips
{
    public class PlayerShip
    {
        private int integrity = 5;
        private int supply = 5;

        public PlayerShip(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Vehicles = PlayerShipResources.None;
            Modules = PlayerShipResources.None;
            Impacts = PlayerShipResources.None;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public int Integrity { get => integrity; set => integrity = (value > 5) ? 5 : (value < 0) ? 0 : value; }
        public string Vehicles { get; set; }
        public string Impacts { get; set; }
        public string Modules { get; set; }
        public int Supply { get => supply; set => supply = (value > 5) ? 5 : (value < 0) ? 0 : value; }
        public bool UseSupply { get; internal set; } = false;
        public IServiceProvider ServiceProvider { get; }
        public string ShipImage { get; private set; }

        public Embed GetEmbed()
        {
            var starshipAsset = ServiceProvider.GetRequiredService<List<IAsset>>()
                .FirstOrDefault(a => a.Name.Equals("Starship", StringComparison.OrdinalIgnoreCase) && a.Game == GameName.Starforged);

            var builder = new EmbedBuilder();
            builder.WithAuthor(PlayerShipResources.ShipCardTitle);
            builder.WithThumbnailUrl(ShipImage);
            builder.WithTitle(Name);
            builder.WithDescription(Description);

            builder.AddField(PlayerShipResources.Modules, this.Modules ?? PlayerShipResources.None, true);
            builder.AddField(PlayerShipResources.Impacts, this.Impacts ?? PlayerShipResources.None, true);
            builder.AddField(PlayerShipResources.Vehicles, this.Vehicles ?? PlayerShipResources.None, true);

            builder.AddField(PlayerShipResources.IntegrityMeter, GetMeterGraphic(Integrity, ":blue_square:", ":ballot_box_with_check:"), true);
            if (this.UseSupply) builder.AddField(PlayerShipResources.SupplyMeter, GetMeterGraphic(Supply, ":green_square:", ":white_check_mark:"), true);

            builder.WithFooter(starshipAsset.AssetAbilities.First().Text.Replace("__", "").Replace("**", ""));

            return builder.Build();
        }

        private string GetMeterGraphic(int value, string emptyGraphic, string fullGraphic)
        {
            var graphic = new string('*', value)
                .PadRight(5, '-')
                .Replace("*", $"{fullGraphic} ")
                .Replace("-", $"{emptyGraphic} ")
                .Trim();
            return $"{value} - {graphic}";
        }

        public PlayerShip PopulateFromEmbed(IEmbed embed)
        {
            if (embed == null) return null;
            if (embed.Author?.Name != PlayerShipResources.ShipCardTitle) return null;

            this.Name = embed.Title;
            this.Description = embed.Description;
            if (embed.Fields.FirstOrDefault(fld => fld.Name == PlayerShipResources.IntegrityMeter) is EmbedField integrityField)
            {
                var match = Regex.Match(integrityField.Value, @"-?\d");
                if (match.Success && int.TryParse(match.Value, out int value)) this.Integrity = value;
            }
            this.ShipImage = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : string.Empty;

            if (embed.Fields.FirstOrDefault(fld => fld.Name == PlayerShipResources.SupplyMeter) is EmbedField supplyField && supplyField.Name != null)
            {
                this.UseSupply = true;
                var match = Regex.Match(supplyField.Value, @"-?\d");
                if (match.Success && int.TryParse(match.Value, out int supply)) this.Supply = supply;
            }

            if (embed.Fields.FirstOrDefault(fld => fld.Name == PlayerShipResources.Impacts) is EmbedField impactsField)
                this.Impacts = impactsField.Value;

            if (embed.Fields.FirstOrDefault(fld => fld.Name == PlayerShipResources.Modules) is EmbedField modulesField)
                this.Modules = modulesField.Value;

            if (embed.Fields.FirstOrDefault(fld => fld.Name == PlayerShipResources.Vehicles) is EmbedField vehiclesField)
                this.Vehicles = vehiclesField.Value;

            return this;
        }

        public PlayerShip IncreaseIntegrity(int amount = 1)
        {
            Integrity += amount;
            return this;
        }
    }
}