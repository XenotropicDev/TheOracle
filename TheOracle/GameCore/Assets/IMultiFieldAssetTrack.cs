using Discord;
using System.Collections;
using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IMultiFieldAssetTrack
    {
        public List<IAssetEmbedField> Fields { get; set; }
    }
}