using System;

namespace TheOracle.GameCore.Assets
{
    public class CountingAssetTrack : ICountingAssetTrack
    {
        public string Name { get; set; }
        public int StartingValue { get; set; }

        public ICountingAssetTrack DeepCopy()
        {
            return (CountingAssetTrack)this.MemberwiseClone(); //Shallow copy is okay until we add some reference type objects
        }
    }
}