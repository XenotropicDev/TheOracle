// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class TriggerRollOptionActionChoiceCustomValue : TriggerRollOptionActionChoice
    {
        [JsonPropertyName("using")]
        public string Using { get => "custom_value"; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
