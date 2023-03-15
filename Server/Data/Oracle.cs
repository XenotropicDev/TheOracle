namespace TheOracle2.Data;
public class Attribute
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Key")]
    public string Key { get; set; }

    [JsonProperty("Values")]
    public List<string> Values { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }
}

public class OracleCategory
{
    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("Usage")]
    public virtual OracleUsage Usage { get; set; }

    [JsonProperty("Oracles")]
    public virtual List<Oracle> Oracles { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Aliases")]
    public List<string> Aliases { get; set; }

    [JsonProperty("Sample Names")]
    public List<string> SampleNames { get; set; }
}

public class Content
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Part of speech")]
    public List<string> PartOfSpeech { get; set; }

    [JsonProperty("Tags")]
    public List<string> Tags { get; set; }
}

public class GameObject
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Object type")]
    public string ObjectType { get; set; }

    [JsonProperty("Requires")]
    public virtual Requires Requires { get; set; }
}

public class MultipleRolls
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Amount")]
    public int Amount { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool AllowDuplicates { get; set; }

    [JsonProperty("Make it worse")]
    public bool MakeItWorse { get; set; }
}

public class Oracle
{
    public override string ToString()
    {
        return Id;
    }

    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Content")]
    public virtual Content Content { get; set; }

    [JsonProperty("Table")]
    public virtual List<Table> Table { get; set; }

    [JsonProperty("Usage")]
    public virtual OracleUsage Usage { get; set; }

    [JsonProperty("Oracles")]
    public virtual List<Oracle> Oracles { get; set; }

    [JsonProperty("Aliases")]
    public List<string> Aliases { get; set; }

    [JsonProperty("Member of")]
    public string MemberOf { get; set; }

    [JsonIgnore]
    public virtual OracleRoot? Parent { get; set; }
}

public class Requires
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Attributes")]
    public virtual List<Attribute> Attributes { get; set; }
}

public class RollTemplate
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Result")]
    public string Result { get; set; }
}

public class OracleRoot
{
    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Aliases")]
    public List<string> Aliases { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Oracles")]
    public virtual List<Oracle> Oracles { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Categories")]
    public virtual List<OracleCategory> Categories { get; set; }
}



public class Suggestions
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Assets")]
    public List<string> Assets { get; set; }

    [JsonProperty("Game objects")]
    public virtual List<GameObject> GameObjects { get; set; }

    [JsonProperty("Oracle rolls")]
    public List<string> OracleRolls { get; set; }
}

public class Table : IComparable<int>
{
    [JsonProperty("Result columns")]
    public virtual List<ResultColumn> ResultColumns { get; set; }

    [JsonProperty("Roll columns")]
    public virtual List<RollColumn> RollColumns { get; set; }

    [JsonProperty("Floor")]
    public int? Floor { get; set; }

    [JsonProperty("Ceiling")]
    public int? Ceiling { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Result")]
    public string Result { get; set; }

    [JsonProperty("Summary")]
    public string Summary { get; set; }

    [JsonProperty("Suggestions")]
    public virtual Suggestions Suggestions { get; set; }

    [JsonProperty("Oracle rolls")]
    public List<string> OracleRolls { get; set; }

    [JsonProperty("Multiple rolls")]
    public virtual MultipleRolls MultipleRolls { get; set; }

    [JsonProperty("Attributes")]
    public virtual List<Attribute> Attributes { get; set; }

    [JsonProperty("Roll template")]
    public virtual RollTemplate RollTemplate { get; set; }

    [JsonProperty("Game objects")]
    public virtual List<GameObject> GameObjects { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Content")]
    public virtual Content Content { get; set; }

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
    public int MaxRolls { get; set; }

    [JsonProperty("Allow duplicates")]
    public bool AllowDuplicates { get; set; }

    [JsonProperty("Initial")]
    public bool? Initial { get; set; }

    [JsonProperty("Suggestions")]
    public virtual Suggestions Suggestions { get; set; }

    [JsonProperty("Sets")]
    public virtual List<Set> Sets { get; set; }

    [JsonProperty("Requires")]
    public virtual Requires Requires { get; set; }

    [JsonProperty("Repeatable")]
    public bool? Repeatable { get; set; }
}

