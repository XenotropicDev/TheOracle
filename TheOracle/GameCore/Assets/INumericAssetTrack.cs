using System.Collections;

namespace TheOracle.GameCore.Assets
{
    public interface INumericAssetTrack
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int StartingNumber { get; set; }
    }
}