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
                    Version = source.Version,
                };
            }
        }

        private IList<IAssetField> assetFields = null;
        public IList<IAssetField> AssetFields { 
            get => assetFields ?? new List<IAssetField>(data.Abilities.Select(a => new AbilityAdapter(a)));
            set => assetFields = value; 
        }
        public string AssetType { get => data.Category; set => data.Category = value; }

        private ICountingAssetTrack countingAssetTrack = null;
        public ICountingAssetTrack CountingAssetTrack 
        { 
            get => countingAssetTrack ?? ((data.Counter != null) ? new CounterAdapter(data.Counter) : null); 
            set => countingAssetTrack = value;
        }
        public string Description { get => Utilities.FormatMarkdownLinks(data.Description); set => data.Description = value; }
        public GameName Game { get; set; }
        public string IconUrl { get; set; }
        public IList<string> InputFields { get => data.Fields; set => data.Fields = value?.ToArray(); }
        public IMultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        public string Name { get => data.Name; set => data.Name = value; }

        private INumericAssetTrack numericAssetTrack = null;
        public INumericAssetTrack NumericAssetTrack {
            get => numericAssetTrack ?? ((data.Track != null) ? new TrackAdapter(data.Track) : null);
            set => numericAssetTrack = value;
        }
        public IList<string> Arguments { get; set; }
        public string UserDescription { get; set; }
        public SourceInfo Source { get; set; }

        public IAsset DeepCopy()
        {
            var asset = (IAsset)this.MemberwiseClone();

            asset.AssetFields = this.AssetFields?.Select(item => item.ShallowCopy()).ToList();
            asset.CountingAssetTrack = this.CountingAssetTrack?.DeepCopy();
            asset.NumericAssetTrack = this.NumericAssetTrack?.DeepCopy();
            asset.InputFields = this.InputFields?.Select(item => item).ToList();
            asset.Arguments = this.Arguments?.Select(item => item).ToList();

            return asset;
        }

        public Embed GetEmbed()
        {
            return Assets.Asset.GetEmbed(this);
        }
    }

    internal class TrackAdapter : INumericAssetTrack
    {
        private readonly Track data;
        public TrackAdapter(Track track)
        {
            data = track;
        }

        public int Min { get; set; } = 0;
        public int Max { get => data.Value; set => data.Value = value; }
        public int ActiveNumber { get => data.StartsAt; set => data.StartsAt = value; }
        public string Name { get => data.Name; set => data.Name = value; }

        public INumericAssetTrack DeepCopy()
        {
            return (INumericAssetTrack)this.MemberwiseClone();
        }
    }

    internal class CounterAdapter : ICountingAssetTrack
    {
        private readonly Counter data;
        public CounterAdapter(Counter counter)
        {
            data = counter;
        }

        public string Name { get => data.Name; set => data.Name = value; }
        public int StartingValue { get => data.StartsAt; set => data.StartsAt = value; }

        public ICountingAssetTrack DeepCopy()
        {
            return (ICountingAssetTrack)this.MemberwiseClone();
        }
    }

    public class AbilityAdapter : IAssetField
    {
        private readonly Ability data;
        public AbilityAdapter(Ability ability)
        {
            data = ability;
        }

        public string Text { get => Utilities.FormatMarkdownLinks(data.Text); set => data.Text = value; }
        public bool Enabled { get => data.Enabled; set => data.Enabled = value; }
        public IEnumerable<string> InputFields { get => data.Fields; set => data.Fields = value?.ToArray(); }

        public IAssetField ShallowCopy()
        {
            return (IAssetField)this.MemberwiseClone();
        }
    }
}