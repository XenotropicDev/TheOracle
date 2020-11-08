using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    internal class MultiFieldAssetTrack : IMultiFieldAssetTrack
    {
        public MultiFieldAssetTrack()
        {
            Fields = new List<IAssetEmbedField>();
        }
        public List<IAssetEmbedField> Fields { get; set; }
    }
}