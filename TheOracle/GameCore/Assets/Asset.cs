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
            AssetAbilities = new List<IAssetAbility>();
            AssetTextInput = new List<string>();
            // AssetRadioSelect = null;
            AssetConditionMeter = null;
            AssetCounter = null;
        }

        public IAsset DeepCopy()
        {
            var asset = (Asset)this.MemberwiseClone();

            asset.AssetAbilities = asset.AssetAbilities.Select(fld => fld.ShallowCopy()).ToList();
            // asset.AssetRadioSelect = asset.AssetRadioSelect?.DeepCopy() ?? null;
            asset.AssetConditionMeter = asset.AssetConditionMeter?.DeepCopy() ?? null;
            asset.AssetCounter = asset.AssetCounter?.DeepCopy() ?? null;
            asset.Arguments = new List<string>();

            return asset;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string UserDescription { get; set; }
        public string IconUrl { get; set; }
        public string Category { get; set; }
        public GameName Game { get; set; }
        public SourceInfo Source { get; set; }

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteListTypeConverter<IAssetAbility, AssetAbility>))]
        public IList<IAssetAbility> AssetAbilities { get; set; }

        // [DefaultValue(null)]
        // [JsonConverter(typeof(ConcreteListTypeConverter<IAssetRadioOption, AssetRadioOption>))]
        // public IList<IAssetRadioOption> AssetRadioSelect { get; set; } = null;

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteTypeConverter<AssetCounter>))]
        public IAssetCounter AssetCounter { get; set; } = null;

        [DefaultValue(null)]
        [JsonConverter(typeof(ConcreteTypeConverter<AssetConditionMeter>))]
        public IAssetConditionMeter AssetConditionMeter { get; set; } = null;

        [DefaultValue(null)]
        public IList<string> AssetTextInput { get; set; }

        [DefaultValue(null)]
        public IList<string> Arguments { get; set; } = new List<string>();

        public override string ToString()
        {
            return Name;
        }

        public static bool IsAssetMessage(IUserMessage message, IServiceProvider services)
        {
            var embed = message.Embeds.FirstOrDefault();

            if (embed.Author.HasValue && embed.Author.Value.Name.Contains(AssetResources.Asset)) return true;
            // for new standard with embed content type being noted in author field

            if (embed.Footer.HasValue && embed.Footer.Value.Text.Contains(AssetResources.Asset)) return true;

            //TODO: this can probably be removed in the future. It's for old asset message support.
            var assets = services.GetRequiredService<List<IAsset>>();
            if (assets.Any(a => embed.Title?.Contains(a.Name) ?? false)) return true;

            return false;
        }

        public static IAsset FromEmbed(IServiceProvider Services, IEmbed embed)
        {
            var game = Utilities.GetGameContainedInString(embed.Footer.Value.Text ?? string.Empty);
            // TODO:
            var asset = Services.GetRequiredService<List<IAsset>>().Single(a => embed.Title.Contains(a.Name) && a.Game == game).DeepCopy();

            for (int i = 0; i < asset.AssetAbilities.Count; i++)
            {
                EmbedField embedField = embed.Fields.FirstOrDefault(fld => fld.Name.Contains((i + 1).ToString()));

                if (embedField.Value == null) embedField = embed.Fields.FirstOrDefault(fld => fld.Value.Contains(asset.AssetAbilities[i].Text)); //match old style assets
                if (embedField.Value == null) continue;

                asset.AssetAbilities[i].Enabled = embedField.Name.Contains(AssetEnabledEmoji) || embedField.Name.Contains(OldAssetEnabledEmoji);
            }

            if (asset.AssetConditionMeter != null)
            {
                var field = embed.Fields.First(f => f.Name == asset.AssetConditionMeter.Name);
                var match = Regex.Match(field.Value, @"__\*\*(\d+)\*\*__");
                int value = 0;
                if (match.Success) int.TryParse(match.Groups[1].Value, out value);
                asset.AssetConditionMeter.ActiveNumber = value;
            }

            if (asset.AssetCounter != null)
            {
                var field = embed.Fields.First(f => f.Name == asset.AssetCounter.Name);
                var match = Regex.Match(field.Value, @"-?\d+");
                int value = 0;
                if (match.Success) int.TryParse(match.Groups[0].Value, out value);
                asset.AssetCounter.StartingValue = value;
            }

            // if (asset.AssetRadioSelect != null)
            // {
            //     foreach (var radioOption in asset.AssetRadioSelect.Options)
            //     {
            //         radioOption.IsActive = embed.Fields.Any(f => radioOption.Name.Contains(f.Name, StringComparison.OrdinalIgnoreCase) && radioOption.ActiveText.Contains(f.Value, StringComparison.OrdinalIgnoreCase));
            //     }
            // }

            foreach (var input in asset.AssetTextInput ?? new List<string>())
            {
                string partialFormated = string.Format(AssetResources.UserTextInput, input, ".*");
                var match = Regex.Match(embed.Description, partialFormated);
                if (match.Success && match.Value.UndoFormatString(AssetResources.UserTextInput, out string[] descValues, true))
                {
                    asset.Arguments.Add(descValues[1]);
                }

                if (!embed.Fields.Any(f => f.Value.Contains(input))) continue;

                EmbedField field = embed.Fields.First(f => f.Value.Contains(input));

                if (!field.Value.UndoFormatString(AssetResources.UserTextInput, out string[] fldValues, true)) continue;
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
            builder.WithAuthor($"Asset: {asset.Category}");
            builder.WithThumbnailUrl(asset.IconUrl);
            builder.WithTitle(asset.Name);

            string fullDesc = string.Empty;
            foreach (var fld in asset.AssetTextInput ?? new List<string>())
            {
                string userVal = (asset.Arguments.Count() - 1 >= nextArgument) ? asset.Arguments.ElementAt(nextArgument) : string.Empty.PadLeft(24, '_');
                fullDesc += String.Format(AssetResources.UserTextInput, fld, userVal) + "\n";
                nextArgument++;
            }
            fullDesc += (fullDesc.Length > 0) ? "\n" + asset.Description : asset.Description;

            if (asset.UserDescription == null || asset.UserDescription.Length == 0) builder.WithDescription(fullDesc);
            else builder.WithDescription(asset.UserDescription);

            int abilityNumber = 0;
            foreach (var abl in asset.AssetAbilities ?? new List<IAssetAbility>())
            {
                abilityNumber++;
                string abilityText = Utilities.FormatMarkdown(abl.Text);
        string label = $"{abilityNumber}. {(abl.Enabled ? AssetEnabledEmoji : AssetDisabledEmoji)}";

                string textInput = string.Empty;
                if (abl.AssetTextInput?.Count() > 0)
                {
                    foreach (var inputItem in abl.AssetTextInput)
                    {
                        string userVal = (asset.Arguments.Count() - 1 >= nextArgument) ? asset.Arguments.ElementAt(nextArgument) : string.Empty.PadLeft(24, '_');
                        textInput += "\n" + String.Format(AssetResources.UserTextInput, inputItem, userVal);
                        nextArgument++;
                    }
                }

                builder.AddField(label, abilityText + textInput);
            }

            // if (asset.AssetRadioSelect?.Options != null)
            // {
            //     foreach (var radioOption in asset.AssetRadioSelect.Options)
            //     {
            //         string text = (radioOption.IsActive) ? radioOption.ActiveText : radioOption.InactiveText;
            //         builder.AddField(radioOption.Name, text, true);
            //     }
            // }

            if (asset.AssetCounter?.Name != null)
            {
                builder.AddField(asset.AssetCounter.Name, asset.AssetCounter.StartingValue);
            }

            if (asset.AssetConditionMeter != null && !(asset.AssetConditionMeter.Max == 0 && asset.AssetConditionMeter.Min == 0))
            {
                string meterText = string.Empty;
                for (int i = asset.AssetConditionMeter.Min; i <= asset.AssetConditionMeter.Max; i++) meterText += $"{i} ";
                meterText = meterText.Trim().Replace(asset.AssetConditionMeter.ActiveNumber.ToString(), $"__**{asset.AssetConditionMeter.ActiveNumber}**__");
                builder.AddField(asset.AssetConditionMeter.Name, meterText);
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
                var ironAssets = JsonConvert.DeserializeObject<AssetInfo>(File.ReadAllText(ironAssetsPath));
                foreach (var asset in ironAssets.Assets)
                {
                    AssetList.Add(new AssetAdapter(asset, GameName.Ironsworn, asset.Source ?? ironAssets.Source));
                }
                // var ironAssets = JsonConvert.DeserializeObject<List<Asset>>(File.ReadAllText(ironAssetsPath));
                // ironAssets.ForEach(a => a.Game = GameCore.GameName.Ironsworn);
                // AssetList.AddRange(ironAssets);
            }
            if (File.Exists(starAssetsPath))
            {
                var starAssets = JsonConvert.DeserializeObject<AssetInfo>(File.ReadAllText(starAssetsPath));
                foreach (var asset in starAssets.Assets)
                {
                    AssetList.Add(new AssetAdapter(asset, GameName.Starforged, asset.Source ?? starAssets.Source));
                }
            }

            return AssetList;
        }
    }
}