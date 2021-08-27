namespace TheOracle.GameCore.Assets
{
    public class AssetConditionMeter : IAssetConditionMeter
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

        public IAssetConditionMeter DeepCopy()
        {
            var track = (AssetConditionMeter)this.MemberwiseClone();

            return track;
        }
    }
}