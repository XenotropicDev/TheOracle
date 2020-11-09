using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAsset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public List<string> InputFields { get; set; }
        public List<AssetField> AssetFields { get; set; }
        public MultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        public CountingAssetTrack CountingAssetTrack { get; set; }
        public NumericAssetTrack NumericAssetTrack { get; set; }

        Embed GetEmbed(string[] arguments);
    }
}