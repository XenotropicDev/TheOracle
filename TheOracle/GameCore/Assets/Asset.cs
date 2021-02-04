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
        public const string OldAssetEnabledEmoji = "\\⏺️";
        public const string AssetEnabledEmoji = "⏺️";
        
        public const string AssetDisabledEmoji = "🟦";

        public Asset()
        {
            AssetFields = new List<AssetField>();
            InputFields = new List<string>();
            MultiFieldAssetTrack = new MultiFieldAssetTrack();
            NumericAssetTrack = new NumericAssetTrack();
            CountingAssetTrack = new CountingAssetTrack();
        }

        public Asset DeepCopy()
        {
            var asset = (Asset)this.MemberwiseClone();

            asset.AssetFields = asset.AssetFields.Select(fld => fld.ShallowCopy()).ToList();
            asset.MultiFieldAssetTrack = asset.MultiFieldAssetTrack?.DeepCopy() ?? null;
            asset.NumericAssetTrack = asset.NumericAssetTrack?.DeepCopy() ?? null;
            asset.CountingAssetTrack = asset.CountingAssetTrack?.DeepCopy() ?? null;
            asset.arguments = new List<string>();

            return asset;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string UserDescription { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public GameName Game { get; set; }

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

        public static bool IsAssetMessage(IUserMessage message, IServiceProvider services)
        {
            return services.GetRequiredService<List<Asset>>().Any(a => message.Embeds.Any(embed => embed.Title.Contains(a.Name)));
        }

        public static Asset FromEmbed(IServiceProvider Services, IEmbed embed)
        {
            var game = Utilities.GetGameContainedInString(embed.Footer.Value.Text ?? string.Empty);
            var asset = Services.GetRequiredService<List<Asset>>().Single(a => embed.Title.Contains(a.Name) && a.Game == game).DeepCopy();

            for (int i = 0; i < asset.AssetFields.Count; i++)
            {
                EmbedField embedField = embed.Fields.FirstOrDefault(fld => fld.Name.Contains((i + 1).ToString()));

                if (embedField.Value == null) embedField = embed.Fields.FirstOrDefault(fld => fld.Value.Contains(asset.AssetFields[i].Text)); //match old style assets
                if (embedField.Value == null) continue;

                asset.AssetFields[i].Enabled = embedField.Name.Contains(AssetEnabledEmoji) || embedField.Name.Contains(OldAssetEnabledEmoji);
            }

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
                var match = Regex.Match(field.Value, @"-?\d+");
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
                string partialFormated = string.Format(AssetResources.UserInputField, input, ".*");
                var match = Regex.Match(embed.Description, partialFormated);
                if (match.Success && match.Value.UndoFormatString(AssetResources.UserInputField, out string[] descValues, true))
                {
                    asset.arguments.Add(descValues[1]);
                }

                if (!embed.Fields.Any(f => f.Value.Contains(input))) continue;

                EmbedField field = embed.Fields.First(f => f.Value.Contains(input));

                if (!field.Value.UndoFormatString(AssetResources.UserInputField, out string[] fldValues, true)) continue;
                asset.arguments.Add(fldValues[1]);
            }

            asset.IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : asset.IconUrl;
            asset.UserDescription = embed.Description;

            return asset;
        }

        public Embed GetEmbed(string[] arguments)
        {
            int nextArgument = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor(AssetType);
            builder.WithThumbnailUrl(IconUrl);
            builder.WithTitle(Name);

            string fullDesc = string.Empty;
            foreach (var fld in InputFields)
            {
                string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(24, '_');
                fullDesc += String.Format(AssetResources.UserInputField, fld, userVal) + "\n";
                nextArgument++;
            }
            fullDesc += (fullDesc.Length > 0) ? "\n" + Description : Description;

            if (UserDescription == null || UserDescription.Length == 0) builder.WithDescription(fullDesc);
            else builder.WithDescription(UserDescription);

            foreach (var fld in AssetFields)
            {
                string label = $"{AssetFields.IndexOf(fld) + 1}. {(fld.Enabled ? AssetEnabledEmoji : AssetDisabledEmoji)}";

                string inputField = string.Empty;
                if (fld.InputFields?.Count > 0)
                {
                    foreach (var inputItem in fld.InputFields)
                    {
                        string userVal = (arguments.Length - 1 >= nextArgument) ? arguments[nextArgument] : string.Empty.PadLeft(24, '_');
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

            builder.WithFooter(String.Format(AssetResources.GameAssetFormat, Game, AssetResources.Asset));

            return builder.Build();
        }
    }
}