// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class TriggerOptionActionChoiceCustom : TriggerOptionActionChoice
    {
        [JsonPropertyName("using")]
        public string Using { get => "custom"; }

        [JsonPropertyName("label")]
        public Label Label { get; set; }

        [JsonPropertyName("value")]
        public sbyte Value { get; set; }
    }
}
