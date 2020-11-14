using Discord;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;

namespace TheOracle.GameCore.Assets
{
    public class Asset : IAsset
    {
        public Asset()
        {
            AssetFields = new List<AssetField>();
            InputFields = new List<string>();
            MultiFieldAssetTrack = new MultiFieldAssetTrack();
            NumericAssetTrack = new NumericAssetTrack();
            CountingAssetTrack = new CountingAssetTrack();
        }

        public Asset ShallowCopy()
        {
            return (Asset)this.MemberwiseClone();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public List<AssetField> AssetFields { get; set; }

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public MultiFieldAssetTrack MultiFieldAssetTrack { get; set; } = null;

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CountingAssetTrack CountingAssetTrack { get; set; } = null;

        [DefaultValue(null)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public NumericAssetTrack NumericAssetTrack { get; set; } = null;

        public List<string> InputFields { get; set; }

        internal List<string> arguments { get; set; } = new List<string>();

        public override string ToString()
        {
            return Name;
        }

        public static Asset FromEmbed(IServiceProvider Services, IEmbed embed)
        {
            var asset = Services.GetRequiredService<List<Asset>>().Single(a => embed.Title.Contains(a.Name)).ShallowCopy();

            if (asset.NumericAssetTrack != null)
            {
                var field = embed.Fields.First(f => f.Name == asset.NumericAssetTrack.Name);
                var match = Regex.Match(field.Value, @"__\*\*(\d+)\*\*__");
                int value = 0;
                if (match.Success) int.TryParse(match.Groups[1].Value, out value);
                asset.NumericAssetTrack.ActiveNumber = value;
            }

            if (asset.CountingAssetTrack != null)
            {
                var field = embed.Fields.First(f => f.Name == asset.CountingAssetTrack.Name);
                var match = Regex.Match(field.Value, @"\d+");
                int value = 0;
                if (match.Success) int.TryParse(match.Groups[0].Value, out value);
                asset.CountingAssetTrack.StartingValue = value;
            }

            if (asset.MultiFieldAssetTrack != null)
            {
                foreach (var assetField in asset.MultiFieldAssetTrack.Fields)
                {
                    assetField.IsActive = embed.Fields.Any(f => assetField.Name.Contains(f.Name, StringComparison.OrdinalIgnoreCase) && assetField.ActiveText.Contains(f.Value, StringComparison.OrdinalIgnoreCase));
                }
            }

            foreach (var input in asset.InputFields)
            {
                if (!embed.Fields.Any(f => f.Value.Contains(input))) continue;

                EmbedField field = embed.Fields.First(f => f.Value.Contains(input));

                if (!field.Value.UndoFormatString(AssetResources.UserInputField, out string[] values, true)) continue;
                asset.arguments.Add(values[1]);
            }

            return asset;
        }

        public Embed GetEmbed(string[] arguments)
        {
            int nextArgument = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor(AssetType, IconUrl);
            builder.WithTitle(Name);

            string fullDesc = string.Empty;
            foreach (var fld in InputFields)
            {
                string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(8);
                fullDesc += String.Format(AssetResources.UserInputField, fld, userVal) + "\n";
                nextArgument++;
            }
            fullDesc += (fullDesc.Length > 0) ? "\n" + Description : Description;

            builder.WithDescription(fullDesc);

            foreach (var fld in AssetFields)
            {
                string label = (fld.Enabled) ? "\\:record_button:" : "\\:blue_square:";

                string inputField = string.Empty;
                if (fld.InputFields?.Count > 0)
                {
                    foreach (var inputItem in fld.InputFields)
                    {
                        string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(8);
                        inputField += "\n" + String.Format(AssetResources.UserInputField, inputItem, userVal);
                        nextArgument++;
                    }
                }

                builder.AddField(label, fld.Text + inputField);
            }

            if (MultiFieldAssetTrack?.Fields != null)
            {
                foreach (var trackItem in MultiFieldAssetTrack.Fields)
                {
                    string text = (trackItem.IsActive) ? trackItem.ActiveText : trackItem.InactiveText;
                    builder.AddField(trackItem.Name, text, true);
                }
            }

            if (CountingAssetTrack?.Name != null)
            {
                builder.AddField(CountingAssetTrack.Name, CountingAssetTrack.StartingValue);
            }

            if (NumericAssetTrack != null && !(NumericAssetTrack.Max == 0 && NumericAssetTrack.Min == 0))
            {
                string trackText = string.Empty;
                for (int i = NumericAssetTrack.Min; i <= NumericAssetTrack.Max; i++) trackText += $"{i} ";
                trackText = trackText.Trim().Replace(NumericAssetTrack.ActiveNumber.ToString(), $"__**{NumericAssetTrack.ActiveNumber}**__");
                builder.AddField(NumericAssetTrack.Name, trackText);
            }

            return builder.Build();
        }
    }
}