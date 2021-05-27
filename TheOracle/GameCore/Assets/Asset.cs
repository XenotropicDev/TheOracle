using Discord;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.Assets
{
    public class Asset : IAsset
    {
        public const string OldAssetEnabledEmoji = "\\⏺️";
        public const string AssetEnabledEmoji = "⏺️";
        
        public const string AssetDisabledEmoji = "🟦";

        public Asset()
        {
            AssetFields = new List<IAssetField>();
            InputFields = new List<string>();
            MultiFieldAssetTrack = null;
            NumericAssetTrack = null;
            CountingAssetTrack = null;
        }

        public IAsset DeepCopy()
        {
            var asset = (Asset)this.MemberwiseClone();

            asset.AssetFields = asset.AssetFields.Select(fld => fld.ShallowCopy()).ToList();
            asset.MultiFieldAssetTrack = asset.MultiFieldAssetTrack?.DeepCopy() ?? null;
            asset.NumericAssetTrack = asset.NumericAssetTrack?.DeepCopy() ?? null;
            asset.CountingAssetTrack = asset.CountingAssetTrack?.DeepCopy() ?? null;
            asset.Arguments = new List<string>();

            return asset;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string UserDescription { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public GameName Game { get; set; }
        public SourceInfo Source { get; set; }

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteListTypeConverter<IAssetField, AssetField>))]
        public IList<IAssetField> AssetFields { get; set; }

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteTypeConverter<MultiFieldAssetTrack>))]
        public IMultiFieldAssetTrack MultiFieldAssetTrack { get; set; } = null;

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteTypeConverter<CountingAssetTrack>))]
        public ICountingAssetTrack CountingAssetTrack { get; set; } = null;

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteTypeConverter<NumericAssetTrack>))]
        public INumericAssetTrack NumericAssetTrack { get; set; } = null;

        [DefaultValue(null)]
        public IList<string> InputFields { get; set; }

        [DefaultValue(null)]
        public IList<string> Arguments { get; set; } = new List<string>();

        public override string ToString()
        {
            return Name;
        }

        public static bool IsAssetMessage(IUserMessage message, IServiceProvider services)
        {
            var embed = message.Embeds.FirstOrDefault();
            if (embed.Footer.HasValue && embed.Footer.Value.Text.Contains(AssetResources.Asset)) return true;

            //TODO: this can probably be removed in the future. It's for old asset message support.
            var assets = services.GetRequiredService<List<IAsset>>();
            if (assets.Any(a => embed.Title?.Contains(a.Name) ?? false)) return true;
            
            return false;
        }

        public static IAsset FromEmbed(IServiceProvider Services, IEmbed embed)
        {
            var game = Utilities.GetGameContainedInString(embed.Footer.Value.Text ?? string.Empty);
            var asset = Services.GetRequiredService<List<IAsset>>().Single(a => embed.Title.Contains(a.Name) && a.Game == game).DeepCopy();

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

            foreach (var input in asset.InputFields ?? new List<string>())
            {
                string partialFormated = string.Format(AssetResources.UserInputField, input, ".*");
                var match = Regex.Match(embed.Description, partialFormated);
                if (match.Success && match.Value.UndoFormatString(AssetResources.UserInputField, out string[] descValues, true))
                {
                    asset.Arguments.Add(descValues[1]);
                }

                if (!embed.Fields.Any(f => f.Value.Contains(input))) continue;

                EmbedField field = embed.Fields.First(f => f.Value.Contains(input));

                if (!field.Value.UndoFormatString(AssetResources.UserInputField, out string[] fldValues, true)) continue;
                asset.Arguments.Add(fldValues[1]);
            }

            asset.IconUrl = embed.Thumbnail.HasValue ? embed.Thumbnail.Value.Url : asset.IconUrl;
            asset.UserDescription = embed.Description;

            return asset;
        }

        public Embed GetEmbed()
        {
            return Asset.GetEmbed(this);
        }

        public static Embed GetEmbed(IAsset asset)
        {
            int nextArgument = 0;

            EmbedBuilder builder = new EmbedBuilder();
            builder.WithAuthor(asset.AssetType);
            builder.WithThumbnailUrl(asset.IconUrl);
            builder.WithTitle(asset.Name);

            string fullDesc = string.Empty;
            foreach (var fld in asset.InputFields ?? new List<string>())
            {
                string userVal = (asset.Arguments.Count() - 1 >= nextArgument) ? asset.Arguments.ElementAt(nextArgument) : string.Empty.PadLeft(24, '_');
                fullDesc += String.Format(AssetResources.UserInputField, fld, userVal) + "\n";
                nextArgument++;
            }
            fullDesc += (fullDesc.Length > 0) ? "\n" + asset.Description : asset.Description;

            if (asset.UserDescription == null || asset.UserDescription.Length == 0) builder.WithDescription(fullDesc);
            else builder.WithDescription(asset.UserDescription);

            int fieldNumber = 0;
            foreach (var fld in asset.AssetFields ?? new List<IAssetField>())
            {
                fieldNumber++;
                string label = $"{fieldNumber}. {(fld.Enabled ? AssetEnabledEmoji : AssetDisabledEmoji)}";

                string inputField = string.Empty;
                if (fld.InputFields?.Count() > 0)
                {
                    foreach (var inputItem in fld.InputFields)
                    {
                        string userVal = (asset.Arguments.Count() - 1 >= nextArgument) ? asset.Arguments.ElementAt(nextArgument) : string.Empty.PadLeft(24, '_');
                        inputField += "\n" + String.Format(AssetResources.UserInputField, inputItem, userVal);
                        nextArgument++;
                    }
                }

                builder.AddField(label, fld.Text + inputField);
            }

            if (asset.MultiFieldAssetTrack?.Fields != null)
            {
                foreach (var trackItem in asset.MultiFieldAssetTrack.Fields)
                {
                    string text = (trackItem.IsActive) ? trackItem.ActiveText : trackItem.InactiveText;
                    builder.AddField(trackItem.Name, text, true);
                }
            }

            if (asset.CountingAssetTrack?.Name != null)
            {
                builder.AddField(asset.CountingAssetTrack.Name, asset.CountingAssetTrack.StartingValue);
            }

            if (asset.NumericAssetTrack != null && !(asset.NumericAssetTrack.Max == 0 && asset.NumericAssetTrack.Min == 0))
            {
                string trackText = string.Empty;
                for (int i = asset.NumericAssetTrack.Min; i <= asset.NumericAssetTrack.Max; i++) trackText += $"{i} ";
                trackText = trackText.Trim().Replace(asset.NumericAssetTrack.ActiveNumber.ToString(), $"__**{asset.NumericAssetTrack.ActiveNumber}**__");
                builder.AddField(asset.NumericAssetTrack.Name, trackText);
            }

            string source = (asset.Source != null) ? asset.Source.ToString() : string.Empty;
            builder.WithFooter(String.Format(AssetResources.FooterFormat, asset.Game, AssetResources.Asset, source));

            return builder.Build();
        }

        public static List<IAsset> LoadAssetList()
        {
            var ironAssetsPath = Path.Combine("IronSworn", "assets.json");
            var starAssetsPath = Path.Combine("StarForged", "Data", "assets.json");
            var AssetList = new List<IAsset>();
            if (File.Exists(ironAssetsPath))
            {
                var ironAssets = JsonConvert.DeserializeObject<List<Asset>>(File.ReadAllText(ironAssetsPath));
                ironAssets.ForEach(a => a.Game = GameCore.GameName.Ironsworn);
                AssetList.AddRange(ironAssets);
            }
            if (File.Exists(starAssetsPath))
            {
                var starAssets = JsonConvert.DeserializeObject<AssetInfo>(File.ReadAllText(starAssetsPath));
                foreach (var asset in starAssets.Assets)
                {
                    AssetList.Add(new AssetAdapter(asset, GameName.Starforged, starAssets.Source));
                }
            }

            return AssetList;
        }
    }
}