using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public class MultiFieldAssetTrack : IMultiFieldAssetTrack
    {
        public MultiFieldAssetTrack()
        {
            Fields = new List<AssetEmbedField>();
        }
        public List<AssetEmbedField> Fields { get; set; }
    }
}