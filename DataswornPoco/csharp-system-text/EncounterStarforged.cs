// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class EncounterStarforged
    {
        [JsonPropertyName("description")]
        public MarkdownString Description { get; set; }

        [JsonPropertyName("drives")]
        public IList<MarkdownString> Drives { get; set; }

        [JsonPropertyName("features")]
        public IList<MarkdownString> Features { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("nature")]
        public EncounterNatureStarforged Nature { get; set; }

        [JsonPropertyName("quest_starter")]
        public MarkdownString QuestStarter { get; set; }

        [JsonPropertyName("rank")]
        public ChallengeRank Rank { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; }

        [JsonPropertyName("summary")]
        public MarkdownString Summary { get; set; }

        [JsonPropertyName("tactics")]
        public IList<MarkdownString> Tactics { get; set; }

        [JsonPropertyName("suggestions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Suggestions? Suggestions { get; set; }

        [JsonPropertyName("variants")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, EncounterVariantStarforged> Variants { get; set; }
    }
}