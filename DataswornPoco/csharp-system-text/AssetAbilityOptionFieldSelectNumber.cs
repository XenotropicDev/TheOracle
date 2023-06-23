﻿// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetAbilityOptionFieldSelectNumber : AssetAbilityOptionField
    {
        [JsonPropertyName("field_type")]
        public string FieldType { get => "select_number"; }

        [JsonPropertyName("choices")]
        public IDictionary<string, AssetAbilityOptionFieldSelectNumberChoice> Choices { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public sbyte? Value { get; set; }
    }
}
