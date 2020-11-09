namespace TheOracle.GameCore.Assets
{
    public class CountingAssetTrack : ICountingAssetTrack
    {
        public string Name { get; set; }
        public int StartingValue { get; set; }
    }
}