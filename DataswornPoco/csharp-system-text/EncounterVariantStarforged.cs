// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class EncounterVariantStarforged
    {
        [JsonPropertyName("description")]
        public MarkdownString Description { get; set; }

        [JsonPropertyName("id")]
        public Id Id { get; set; }

        [JsonPropertyName("name")]
        public Label Name { get; set; }

        [JsonPropertyName("nature")]
        public EncounterNatureStarforged Nature { get; set; }

        [JsonPropertyName("rank")]
        public ChallengeRank Rank { get; set; }
    }
}
