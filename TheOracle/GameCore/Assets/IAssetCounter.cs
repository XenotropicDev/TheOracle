namespace TheOracle.GameCore.Assets
{
    public interface IAssetCounter
    {
        public string Name { get; set; }
        public int StartingValue { get; set; }

        IAssetCounter DeepCopy();
    }
}