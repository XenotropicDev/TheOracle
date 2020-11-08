namespace TheOracle.GameCore.Assets
{
    public class NumericAssetTrack : INumericAssetTrack
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int StartingNumber { get; set; }
    }
}