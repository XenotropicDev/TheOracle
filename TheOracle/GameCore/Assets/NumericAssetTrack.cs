namespace TheOracle.GameCore.Assets
{
    public class NumericAssetTrack : INumericAssetTrack
    {
        private int activeNumber;

        public string Name { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public int ActiveNumber
        {
            get => activeNumber;
            set
            {
                if (value >= Min && value <= Max) activeNumber = value;
            }
        }
    }
}