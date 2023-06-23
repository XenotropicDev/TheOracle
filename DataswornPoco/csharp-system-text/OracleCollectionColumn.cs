// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Text.Json.Serialization;

namespace Dataforged
{
    public class OracleCollectionColumn
    {
        [JsonPropertyName("content_type")]
        public OracleColumnContentType ContentType { get; set; }

        [JsonPropertyName("table_key")]
        public string TableKey { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Color? Color { get; set; }

        [JsonPropertyName("label")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Label { get; set; }
    }
}
