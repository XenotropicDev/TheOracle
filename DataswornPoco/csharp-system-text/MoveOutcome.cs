// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class MoveOutcome
    {
        [JsonPropertyName("text")]
        public MarkdownString Text { get; set; }

        [JsonPropertyName("count_as")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MoveOutcomeType? CountAs { get; set; }

        [JsonPropertyName("reroll")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public MoveReroll? Reroll { get; set; }
    }
}
