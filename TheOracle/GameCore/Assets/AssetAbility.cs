using System.Collections.Generic;
using System.Linq;

namespace TheOracle.GameCore.Assets
{
    public class AssetAbility : IAssetAbility
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<string> AssetTextInput { get; set; }

        public IAssetAbility ShallowCopy()
        {
            var instance = (AssetAbility)this.MemberwiseClone();
            if (AssetTextInput != null) instance.AssetTextInput = AssetTextInput.Select(s => s).ToList();

            return instance;
        }
    }
}