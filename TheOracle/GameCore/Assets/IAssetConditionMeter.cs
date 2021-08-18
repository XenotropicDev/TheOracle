using System.Collections;

namespace TheOracle.GameCore.Assets
{
    public interface IAssetConditionMeter
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int ActiveNumber { get; set; }
        string Name { get; set; }

        IAssetConditionMeter DeepCopy();
    }
}