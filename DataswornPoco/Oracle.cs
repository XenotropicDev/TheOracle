using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace TheOracle2.Data;
public class Attribute
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Key")]
    public string? Key { get; set; }

    [JsonProperty("Values")]
    public ObservableCollection<string>? Values { get; set; } = new();

    [JsonProperty("Value")]
    public string? Value { get; set; }
}

public class OracleCategory
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Category")]
    public string? Category { get; set; }

    [JsonProperty("Usage")]
    public virtual OracleUsage? Usage { get; set; }

    [JsonProperty("Oracles")]
    public virtual ObservableCollection<Oracle>? Oracles { get; set; }

    [JsonProperty("Description")]
    public string? Description { get; set; }

    [JsonProperty("Aliases")]
    public ObservableCollection<string>? Aliases { get; set; } = new();

    [JsonProperty("Sample Names")]
    public ObservableCollection<string>? SampleNames { get; set; } = new();
}

public class Content
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Part of speech")]
    public ObservableCollection<string>? PartOfSpeech { get; set; } = new();

    [JsonProperty("Tags")]
    public ObservableCollection<string>? Tags { get; set; } = new();
}

public class GameObject
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Object type")]
    public string? ObjectType { get; set; }

    [JsonProperty("Requires")]
    public virtual Requires? Requires { get; set; }
}

public class MultipleRolls
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Amount")]
    public int? Amount { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool? AllowDuplicates { get; set; }

    [JsonProperty("Make it worse")]
    public bool? MakeItWorse { get; set; }
}

public class Oracle
{
    [JsonIgnore]
    public uint Id { get; set; }

    public override string ToString()
    {
        return JsonId;
    }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Category")]
    public string? Category { get; set; }

    [JsonProperty("Description")]
    public string? Description { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Content")]
    public virtual Content? Content { get; set; }

    [JsonProperty("Table")]
    public virtual ObservableCollection<Table>? Table { get; set; } = new();

    [JsonProperty("Usage")]
    public virtual OracleUsage? Usage { get; set; }

    [JsonProperty("Oracles")]
    public virtual ObservableCollection<Oracle>? Oracles { get; set; } = new();

    [JsonProperty("Aliases")]
    public ObservableCollection<string>? Aliases { get; set; } = new();

    [JsonProperty("Member of")]
    public string? MemberOf { get; set; }

    [JsonIgnore]
    public virtual OracleRoot? Parent { get; set; }
}

public class Requires
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Attributes")]
    public virtual ObservableCollection<Attribute>? Attributes { get; set; }
}

public class RollTemplate
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Result")]
    public string? Result { get; set; }
}

public class OracleRoot
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Aliases")]
    public ObservableCollection<string>? Aliases { get; set; } = new();

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Oracles")]
    public virtual ObservableCollection<Oracle>? Oracles { get; set; } = new();

    [JsonProperty("Description")]
    public string? Description { get; set; }

    [JsonProperty("Categories")]
    public virtual ObservableCollection<OracleCategory>? Categories { get; set; } = new();
}

public class Suggestions
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Assets")]
    public ObservableCollection<string>? Assets { get; set; }

    [JsonProperty("Game objects")]
    public virtual ObservableCollection<GameObject>? GameObjects { get; set; } = new();

    [JsonProperty("Oracle rolls")] //ItemConverterType = typeof(ReferenceStringConverter)
    public virtual ObservableCollection<string>? OracleRolls { get; set; } = new();
}

public class Table : IComparable<int>
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Result columns")]
    public virtual ObservableCollection<ResultColumn>? ResultColumns { get; set; } = new();

    [JsonProperty("Roll columns")]
    public virtual ObservableCollection<RollColumn>? RollColumns { get; set; } = new();

    [JsonProperty("Floor")]
    public int? Floor { get; set; }

    [JsonProperty("Ceiling")]
    public int? Ceiling { get; set; }

    [JsonProperty("$id")]
    public string? JsonId { get; set; }

    [JsonProperty("Result")]
    public string? Result { get; set; }

    [JsonProperty("Summary")]
    public string? Summary { get; set; }

    [JsonProperty("Suggestions")]
    public virtual Suggestions? Suggestions { get; set; }

    [JsonProperty("Oracle rolls")]
    public ObservableCollection<string>? OracleRolls { get; set; } = new();

    [JsonProperty("Multiple rolls")]
    public virtual MultipleRolls? MultipleRolls { get; set; }

    [JsonProperty("Attributes")]
    public virtual ObservableCollection<Attribute>? Attributes { get; set; } = new();

    [JsonProperty("Roll template")]
    public virtual RollTemplate? RollTemplate { get; set; }

    [JsonProperty("Game objects")]
    public virtual ObservableCollection<GameObject>? GameObjects { get; set; } = new();

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Content")]
    public virtual Content? Content { get; set; }

    public int CompareTo(int other)
    {
        if (other > Ceiling) return -1;
        if (other >= Floor && other <= Ceiling) return 0;
        if (other < Ceiling) return 1;
        return -1;
    }
}

public class OracleUsage
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Max rolls")]
    public int? MaxRolls { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool? AllowDuplicates { get; set; }

    [JsonProperty("Initial")]
    public bool? Initial { get; set; }

    [JsonProperty("Suggestions")]
    public virtual Suggestions? Suggestions { get; set; }

    [JsonProperty("Sets")]
    public virtual ObservableCollection<Set>? Sets { get; set; } = new();

    [JsonProperty("Requires")]
    public virtual Requires? Requires { get; set; }

    [JsonProperty("Repeatable")]
    public bool? Repeatable { get; set; }
}

