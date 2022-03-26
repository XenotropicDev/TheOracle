namespace TheOracle.GameCore.Oracle.DataSworn
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public partial class GlossaryRoot
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Source")]
        public Source Source { get; set; }

        [JsonProperty("Terms")]
        public IList<Term> Terms { get; set; }

        [JsonProperty("Description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }

    public partial class Term
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Color", NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Applied by", NullValueHandling = NullValueHandling.Ignore)]
        public AppliedBy AppliedBy { get; set; }

        [JsonProperty("Removed by", NullValueHandling = NullValueHandling.Ignore)]
        public RemovedBy RemovedBy { get; set; }

        [JsonProperty("Effects", NullValueHandling = NullValueHandling.Ignore)]
        public Effect Effects { get; set; }

        [JsonProperty("Applies to", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> AppliesTo { get; set; }

        [JsonProperty("Effect", NullValueHandling = NullValueHandling.Ignore)]
        public Effect Effect { get; set; }
    }

    public partial class AppliedBy
    {
        [JsonProperty("Moves")]
        public IList<string> Moves { get; set; }
    }

    public partial class Effect
    {
        [JsonProperty("Forbid Increase")]
        public IList<string> ForbidIncrease { get; set; }
    }

    public partial class RemovedBy
    {
        [JsonProperty("Moves", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Moves { get; set; }

        [JsonProperty("Quest", NullValueHandling = NullValueHandling.Ignore)]
        public Quest Quest { get; set; }
    }

    public partial class Quest
    {
        [JsonProperty("Rank")]
        public IList<string> Rank { get; set; }
    }
}