﻿// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetAbilityOptionFieldText : AssetAbilityOptionField
    {
        [JsonPropertyName("field_type")]
        public string FieldType { get => "text"; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string Value { get; set; }
    }
}
