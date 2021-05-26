using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace TheOracle.GameCore.Assets
{
    public class AssetField : IAssetField
    {
        public string Text { get; set; }
        public bool Enabled { get; set; }
        public IEnumerable<string> InputFields { get; set; }

        public IAssetField ShallowCopy()
        {
            var instance = (AssetField)this.MemberwiseClone();
            if (InputFields != null) instance.InputFields = InputFields.Select(s => s).ToList();

            return instance;
        }
    }
}