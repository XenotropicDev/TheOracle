using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAssetField
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
        IEnumerable<string> InputFields { get; set; }

        IAssetField ShallowCopy();
    }
}