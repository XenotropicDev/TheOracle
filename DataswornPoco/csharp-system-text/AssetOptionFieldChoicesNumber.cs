// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetOptionFieldChoicesNumber : AssetOptionField
    {
        [JsonPropertyName("field_type")]
        public string FieldType { get => "choices_number"; }

        [JsonPropertyName("choices")]
        public IDictionary<string, AssetOptionFieldChoicesNumberChoice> Choices { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }
    }
}
