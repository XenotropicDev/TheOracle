// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class TriggerOptionActionChoiceRef : TriggerOptionActionChoice
    {
        [JsonPropertyName("using")]
        public string Using { get => "ref"; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("ref")]
        public string Ref { get; set; }
    }
}
