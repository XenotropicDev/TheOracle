using Discord;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAsset
    {
        IList<IAssetAbility> AssetAbilities { get; set; }
        string Category { get; set; }
        IAssetCounter AssetCounter { get; set; }
        string Description { get; set; }
        GameName Game { get; set; }
        string IconUrl { get; set; }
        IList<string> AssetTextInput { get; set; }

        // IAssetRadioSelect AssetRadioSelect { get; set; }
        string Name { get; set; }

        IAssetConditionMeter AssetConditionMeter { get; set; }
        IList<string> Arguments { get; set; }
        string UserDescription { get; set; }
        SourceInfo Source { get; set; }

        IAsset DeepCopy();

        Embed GetEmbed();
    }
}