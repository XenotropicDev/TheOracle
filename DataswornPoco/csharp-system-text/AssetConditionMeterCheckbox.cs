// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetConditionMeterCheckbox
    {
        [JsonPropertyName("field_type")]
        public AssetConditionMeterCheckboxFieldType FieldType { get; set; }

        [JsonPropertyName("id")]
        public AssetConditionMeterControlFieldId Id { get; set; }

        [JsonPropertyName("label")]
        public Label Label { get; set; }

        [JsonPropertyName("value")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? Value { get; set; }
    }
}
