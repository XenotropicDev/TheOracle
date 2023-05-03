using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TheOracle2.Data;

public class Move
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Optional")]
    public bool Optional { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger? Trigger { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("Oracles")]
    public ObservableCollection<string> Oracles { get; set; } = new();

    [JsonProperty("Outcomes")]
    public virtual Outcomes? Outcomes { get; set; }

    [JsonProperty("Progress Move")]
    public bool? ProgressMove { get; set; }

    [JsonProperty("Variant of")]
    public string? VariantOf { get; set; }

    [JsonIgnore]
    public virtual MoveRoot? Parent { get; set; }
}

public class MoveRoot
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Optional")]
    public bool Optional { get; set; }

    [JsonProperty("Moves")]
    public virtual ObservableCollection<Move> Moves { get; set; } = new();
}

