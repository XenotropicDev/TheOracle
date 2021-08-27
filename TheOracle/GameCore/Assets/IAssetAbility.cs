using System.Collections.Generic;

namespace TheOracle.GameCore.Assets
{
    public interface IAssetAbility
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
        IEnumerable<string> AssetTextInput { get; set; }

        IAssetAbility ShallowCopy();
    }
}