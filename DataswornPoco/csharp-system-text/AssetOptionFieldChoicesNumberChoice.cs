// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetOptionFieldChoicesNumberChoice
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public sbyte Value { get; set; }

        [JsonPropertyName("selected")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? Selected { get; set; }
    }
}
