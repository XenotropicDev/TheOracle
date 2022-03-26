namespace TheOracle.GameCore.Oracle.DataSworn
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class TruthRoot
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Source")]
        public Source Source { get; set; }

        [JsonProperty("Setting Truths")]
        public IList<SettingTruth> SettingTruths { get; set; }
    }

    public partial class SettingTruth
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Table")]
        public IList<SettingTruthTable> Table { get; set; }

        [JsonProperty("Character")]
        public string Character { get; set; }
    }

    public partial class SettingTruthTable
    {
        [JsonProperty("Chance")]
        public long Chance { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Details")]
        public string Details { get; set; }

        [JsonProperty("Table", NullValueHandling = NullValueHandling.Ignore)]
        public IList<ChanceDescriptionStub> Table { get; set; }

        [JsonProperty("Quest Starter")]
        public string QuestStarter { get; set; }
    }

    public partial class ChanceDescriptionStub
    {
        [JsonProperty("Chance")]
        public long Chance { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    //public partial class Source
    //{
    //    [JsonProperty("Name")]
    //    public string Name { get; set; }

    //    [JsonProperty("Date")]
    //    public string Date { get; set; }
    //}
}