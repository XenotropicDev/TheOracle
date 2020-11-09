using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public class AssetField : IAssetField
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public List<string> InputFields { get; set; }
    }
}