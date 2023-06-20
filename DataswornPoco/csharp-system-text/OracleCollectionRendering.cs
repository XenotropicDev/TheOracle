// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class OracleCollectionRendering
    {
        [JsonPropertyName("columns")]
        public IDictionary<string, OracleCollectionColumn> Columns { get; set; }

        [JsonPropertyName("color")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Color? Color { get; set; }

        [JsonPropertyName("style")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public OracleCollectionStyle? Style { get; set; }
    }
}
