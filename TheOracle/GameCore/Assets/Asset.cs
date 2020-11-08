using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public class Asset : IAsset
    {
        public Asset()
        {
            AssetFields = new List<IAssetField>();
            InputFields = new List<string>();
            MultiFieldAssetTrack = new MultiFieldAssetTrack();
            NumericAssetTrack = new NumericAssetTrack();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public string AssetType { get; set; }
        public List<IAssetField> AssetFields { get; set; }
        public IMultiFieldAssetTrack MultiFieldAssetTrack { get; set; }
        public INumericAssetTrack NumericAssetTrack { get; set; }
        public List<string> InputFields { get; set; }
    }
}