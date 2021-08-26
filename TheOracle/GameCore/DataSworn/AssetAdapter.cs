using Discord;
using System.Collections.Generic;
using System.Linq;
using TheOracle.BotCore;
using TheOracle.GameCore.Assets;

namespace TheOracle.GameCore.Oracle.DataSworn
{
    internal class AssetAdapter : IAsset
    {
        private readonly Asset data;

        public AssetAdapter(Asset asset, GameName Game, Source source)
        {
            this.data = asset;
            this.Game = Game;

            if (source != null)
            {
                this.Source = new SourceInfo()
                {
                    Name = source.Name,
                    Page = source.Page,
                    Date = source.Date,
                    Url = source.Url ?? null
                };
            }
        }

        private IList<IAssetAbility> assetAbilities = null;
        public IList<IAssetAbility> AssetAbilities {
            get => assetAbilities ?? new List<IAssetAbility>(data.Abilities.Select(a => new AbilityAdapter(a)));
            set => assetAbilities = value;
        }
        public string Category { get => data.Category; set => data.Category = value; }

        private IAssetCounter assetCounter = null;
        public IAssetCounter AssetCounter
        {
            get => assetCounter ?? ((data.Counter != null) ? new CounterAdapter(data.Counter) : null);
            set => assetCounter = value;
        }
        public string Description { get => Utilities.FormatMarkdown(data.Description); set => data.Description = value; }
        public GameName Game { get; set; }
        public string IconUrl { get; set; }
        public IList<string> AssetTextInput { get => data.TextInput; set => data.TextInput = value?.ToArray(); }
        // private IAssetRadioSelect assetRadioSelect = null;
        // public IAssetRadioSelect AssetRadioSelect { get; set; }
        public string Name { get => data.Name; set => data.Name = value; }

        private IAssetConditionMeter assetConditionMeter = null;
        public IAssetConditionMeter AssetConditionMeter {
            get => assetConditionMeter ?? ((data.ConditionMeter != null) ? new MeterAdapter(data.ConditionMeter) : null);
            set => assetConditionMeter = value;
        }
        public IList<string> Arguments { get; set; }
        public string UserDescription { get; set; }
        public SourceInfo Source { get; set; }

        public IAsset DeepCopy()
        {
            var asset = (IAsset)this.MemberwiseClone();

            asset.AssetAbilities = this.AssetAbilities?.Select(item => item.ShallowCopy()).ToList() ?? new List<IAssetAbility>();
            asset.AssetCounter = this.AssetCounter?.DeepCopy();
            asset.AssetConditionMeter = this.AssetConditionMeter?.DeepCopy();
            asset.AssetTextInput = this.AssetTextInput?.Select(item => item).ToList() ?? new List<string>();
            asset.Arguments = this.Arguments?.Select(item => item).ToList() ?? new List<string>();

            return asset;
        }

        public Embed GetEmbed()
        {
            return Assets.Asset.GetEmbed(this);
        }
    }

    internal class MeterAdapter : IAssetConditionMeter
    {
        private readonly ConditionMeter data;
        public MeterAdapter(ConditionMeter meter)
        {
            data = meter;
        }

        public int Min { get; set; } = 0;
        public int Max { get => data.Value; set => data.Value = value; }
        public int ActiveNumber { get => data.StartsAt; set => data.StartsAt = value; }
        public string Name { get => data.Name; set => data.Name = value; }

        public IAssetConditionMeter DeepCopy()
        {
            return (IAssetConditionMeter)this.MemberwiseClone();
        }
    }

    internal class CounterAdapter : IAssetCounter
    {
        private readonly Counter data;
        public CounterAdapter(Counter counter)
        {
            data = counter;
        }

        public string Name { get => data.Name; set => data.Name = value; }
        public int StartingValue { get => data.StartsAt; set => data.StartsAt = value; }

        public IAssetCounter DeepCopy()
        {
            return (IAssetCounter)this.MemberwiseClone();
        }
    }

    public class AbilityAdapter : IAssetAbility
    {
        private Ability data;
        public AbilityAdapter(Ability ability)
        {
            data = ability;
        }

        public string Text { get => Utilities.FormatMarkdown(data.Text); set => data.Text = value; }
        public bool Enabled { get => data.Enabled; set => data.Enabled = value; }
        public IEnumerable<string> AssetTextInput { get => data.TextInput; set => data.TextInput = value?.ToArray(); }

        public IAssetAbility ShallowCopy()
        {
            var clone = (AbilityAdapter)this.MemberwiseClone();
            clone.data = this.data.DeepCopy();
            return clone;
        }
    }
}