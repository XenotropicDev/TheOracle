namespace TheOracle.GameCore.Oracle.DataSworn
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class EncountersRoot
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Source")]
        public Source Source { get; set; }

        [JsonProperty("Encounters")]
        public IList<Encounter> Encounters { get; set; }
    }

    public partial class Encounter
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Rank")]
        public long Rank { get; set; }

        [JsonProperty("Features")]
        public IList<string> Features { get; set; }

        [JsonProperty("Drives")]
        public IList<string> Drives { get; set; }

        [JsonProperty("Tactics")]
        public IList<string> Tactics { get; set; }

        [JsonProperty("Variants", NullValueHandling = NullValueHandling.Ignore)]
        public IList<Variant> Variants { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Quest Starter")]
        public string QuestStarter { get; set; }
    }

    public partial class Variant
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Rank")]
        public long Rank { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }
}