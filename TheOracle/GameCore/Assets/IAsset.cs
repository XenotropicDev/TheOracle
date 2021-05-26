using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAsset
    {
        IList<IAssetField> AssetFields { get; set; }
        string AssetType { get; set; }
        ICountingAssetTrack CountingAssetTrack { get; set; }
        string Description { get; set; }
        GameName Game { get; set; }
        string IconUrl { get; set; }
        IList<string> InputFields { get; set; }
        IMultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        string Name { get; set; }
        INumericAssetTrack NumericAssetTrack { get; set; }
        IList<string> Arguments { get; set; }
        string UserDescription { get; set; }
        SourceInfo Source { get; set; }

        IAsset DeepCopy();
        Embed GetEmbed();
    }
}