using System;

namespace TheOracle.GameCore.Assets
{
    public class AssetCounter : IAssetCounter
    {
        public string Name { get; set; }
        public int StartingValue { get; set; }

        public IAssetCounter DeepCopy()
        {
            return (AssetCounter)this.MemberwiseClone(); //Shallow copy is okay until we add some reference type objects
        }
    }
}