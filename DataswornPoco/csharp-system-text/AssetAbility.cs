// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class AssetAbility
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("id")]
        public AssetAbilityId Id { get; set; }

        [JsonPropertyName("text")]
        public MarkdownString Text { get; set; }

        [JsonPropertyName("controls")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, AssetAbilityControlField> Controls { get; set; }

        [JsonPropertyName("extend_asset")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public AssetExtension? ExtendAsset { get; set; }

        [JsonPropertyName("extend_moves")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IList<MoveExtension> ExtendMoves { get; set; }

        [JsonPropertyName("moves")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, Move> Moves { get; set; }

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Label? Name { get; set; }

        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public IDictionary<string, AssetAbilityOptionField> Options { get; set; }
    }
}
