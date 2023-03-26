using Newtonsoft.Json;

namespace TheOracle2.Data;

public class Display
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Title")]
    public string? Title { get; set; }

    [JsonProperty("Table")]
    public virtual DisplayTable? Table { get; set; }

    [JsonProperty("Icon")]
    public string? Icon { get; set; }

    [JsonProperty("Images")]
    public List<string>? Images { get; set; }

    [JsonProperty("Column of")]
    public string? ColumnOf { get; set; }

    [JsonProperty("Color")]
    public string? Color { get; set; }
}

public class DisplayTable
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Result columns")]
    public virtual List<ResultColumn> Resultcolumns { get; set; } = new();

    [JsonProperty("Roll columns")]
    public virtual List<RollColumn> Rollcolumns { get; set; } = new();
}

public class ResultColumn
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Label")]
    public string Label { get; set; }

    [JsonProperty("Use content from")]
    public string UseContentFrom { get; set; }

    [JsonProperty("Key")]
    public string Key { get; set; }
}

public class RollColumn
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Label")]
    public string Label { get; set; }

    [JsonProperty("Use content from")]
    public string UseContentFrom { get; set; }
}

public class Set
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Key")]
    public string Key { get; set; }

    [JsonProperty("Value")]
    public string Value { get; set; }
}

public class Source
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Title")]
    public string Title { get; set; }

    [JsonProperty("Url")]
    public string Url { get; set; }

    [JsonProperty("Date")]
    public string Date { get; set; }

    [JsonProperty("Page")]
    public int? Page { get; set; }

    [JsonProperty("Authors")]
    public List<string>? Authors { get; set; }
}

public class Miss
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Reroll")]
    public virtual Reroll Reroll { get; set; }

    [JsonProperty("With a Match")]
    public virtual WithAMatch WithAMatch { get; set; }

    [JsonProperty("Count as")]
    public string CountAs { get; set; }
}

public class CustomStat
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Options")]
    public virtual List<Option> Options { get; set; }
}

public class Trigger
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("By")]
    public virtual By By { get; set; }

    [JsonProperty("Options")]
    public virtual List<Option> Options { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }
}

public class StrongHit
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("With a Match")]
    public virtual WithAMatch WithAMatch { get; set; }

    [JsonProperty("Reroll")]
    public virtual Reroll Reroll { get; set; }

    [JsonProperty("Count as")]
    public string CountAs { get; set; }
}

public class WeakHit
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Reroll")]
    public virtual Reroll Reroll { get; set; }

    [JsonProperty("Count as")]
    public string CountAs { get; set; }

    [JsonProperty("In Control")]
    public bool? InControl { get; set; }
}

public class WithAMatch
{
    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }
}

public class Option
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Set")]
    public virtual List<Set> Set { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("Roll type")]
    public string RollType { get; set; }

    [JsonProperty("Method")]
    public string Method { get; set; }

    [JsonProperty("Using")]
    public List<string> Using { get; set; }

    [JsonProperty("Custom stat")]
    public virtual CustomStat CustomStat { get; set; }

    [JsonProperty("Value")]
    public int Value { get; set; }
}

public class Outcomes
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Strong Hit")]
    public virtual StrongHit StrongHit { get; set; }

    [JsonProperty("Weak Hit")]
    public virtual WeakHit WeakHit { get; set; }

    [JsonProperty("Miss")]
    public virtual Miss Miss { get; set; }
}
