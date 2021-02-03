using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAsset
    {
        List<AssetField> AssetFields { get; set; }
        string AssetType { get; set; }
        CountingAssetTrack CountingAssetTrack { get; set; }
        string Description { get; set; }
        GameName Game { get; set; }
        string IconUrl { get; set; }
        List<string> InputFields { get; set; }
        MultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        string Name { get; set; }
        NumericAssetTrack NumericAssetTrack { get; set; }

        Asset DeepCopy();
        Embed GetEmbed(string[] arguments);
    }
}