namespace TheOracle.GameCore.Assets
{
    public interface ICountingAssetTrack
    {
        public string Name { get; set; }
        public int StartingValue { get; set; }
    }
}