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
        public List<IAssetField> AssetFields { get; set; }
        public IMultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        public INumericAssetTrack NumericAssetTrack { get; set; }
    }
}