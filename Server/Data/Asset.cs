namespace TheOracle2.Data;

public class Ability
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("Alter Moves")]
    public virtual List<AlterMove> AlterMoves { get; set; }

    [JsonProperty("Moves")]
    public virtual List<AssetMove> Moves { get; set; }

    [JsonProperty("Alter Momentum")]
    public virtual AlterMomentum AlterMomentum { get; set; }

    [JsonProperty("Alter Properties")]
    public virtual AlterProperties AlterProperties { get; set; }

    [JsonProperty("Inputs")]
    public virtual List<Input> Inputs { get; set; }
}

public class AlterMomentum
{
    [JsonProperty("Burn")]
    public virtual List<Burn> Burn { get; set; }

    [JsonProperty("Reset")]
    public virtual List<Reset> Reset { get; set; }
}

public class AlterMove
{
    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Moves")]
    public List<string> Moves { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger Trigger { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("Outcomes")]
    public virtual Outcomes Outcomes { get; set; }

    [JsonProperty("Alters")]
    public List<string> Alters { get; set; }
}

public class AlterProperties
{
    [JsonProperty("Attachments")]
    public virtual Attachments Attachments { get; set; }

    [JsonProperty("States")]
    public virtual List<State> States { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter ConditionMeter { get; set; }
}

public class Asset
{
    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Asset Type")]
    public string AssetType { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Usage")]
    public virtual AssetUsage? Usage { get; set; }

    [JsonProperty("Attachments")]
    public virtual Attachments? Attachments { get; set; }

    [JsonProperty("Inputs")]
    public virtual List<Input>? Inputs { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter ConditionMeter { get; set; }

    [JsonProperty("Abilities")]
    public virtual List<Ability> Abilities { get; set; }

    [JsonProperty("States")]
    public virtual List<State> States { get; set; }

    [JsonProperty("Requirement")]
    public string Requirement { get; set; }

    [JsonProperty("Aliases")]
    public List<string> Aliases { get; set; }

    [JsonIgnore]
    public virtual AssetRoot? Parent { get; set; }
}

public class Attachments
{
    [JsonProperty("Asset Types")]
    public List<string> AssetTypes { get; set; }

    [JsonProperty("Max")]
    public virtual object Max { get; set; }
}

public class Burn
{
    [JsonProperty("Trigger")]
    public virtual Trigger Trigger { get; set; }

    [JsonProperty("Effect")]
    public virtual Effect Effect { get; set; }

    [JsonProperty("Outcomes")]
    public List<string> Outcomes { get; set; }
}

public class By
{
    [JsonProperty("Player")]
    public bool Player { get; set; }

    [JsonProperty("Ally")]
    public bool Ally { get; set; }
}

public class ConditionMeter
{
    [JsonProperty("Min")]
    public int Min { get; set; }

    [JsonProperty("Value")]
    public int Value { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Max")]
    public int Max { get; set; }

    [JsonProperty("Conditions")]
    public List<string> Conditions { get; set; }

    [JsonProperty("Aliases")]
    public List<string> Aliases { get; set; }
}`

public class Effect
{
    [JsonProperty("Text")]
    public string Text { get; set; }
}

public class Input
{
    [JsonProperty("Input Type")]
    public virtual AssetInput InputType { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Adjustable")]
    public bool Adjustable { get; set; }

    [JsonProperty("Sets")]
    public virtual List<Set> Sets { get; set; }

    [JsonProperty("Options")]
    public virtual List<Option> Options { get; set; }

    [JsonProperty("Step")]
    public int Step { get; set; }

    [JsonProperty("Min")]
    public int Min { get; set; }

    [JsonProperty("Max")]
    public virtual object Max { get; set; }

    [JsonProperty("Value")]
    public int Value { get; set; }

    [JsonProperty("Clock Type")]
    public string ClockType { get; set; }

    [JsonProperty("Segments")]
    public int? Segments { get; set; }

    [JsonProperty("Filled")]
    public int? Filled { get; set; }
}

public enum AssetInput
{
    Clock,
    Select,
    Number,
    Text
}

public class AssetMove
{
    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("Category")]
    public string Category { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Optional")]
    public bool Optional { get; set; }

    [JsonProperty("Asset")]
    public string Asset { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger Trigger { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }

    [JsonProperty("Outcomes")]
    public virtual Outcomes Outcomes { get; set; }

    [JsonProperty("Progress Move")]
    public bool? ProgressMove { get; set; }
}

public class Reroll
{
    [JsonProperty("Dice")]
    public string Dice { get; set; }

    [JsonProperty("Text")]
    public string Text { get; set; }
}

public class Reset
{
    [JsonProperty("Trigger")]
    public virtual Trigger Trigger { get; set; }

    [JsonProperty("Value")]
    public int Value { get; set; }
}

public class AssetRoot
{
    [JsonProperty("Source")]
    public virtual Source Source { get; set; }

    [JsonProperty("$id")]
    public string Id { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Description")]
    public string Description { get; set; }

    [JsonProperty("Display")]
    public virtual Display Display { get; set; }

    [JsonProperty("Usage")]
    public virtual AssetUsage Usage { get; set; }

    [JsonProperty("Assets")]
    public virtual List<Asset> Assets { get; set; }
}

public class State
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("Disables asset")]
    public bool DisablesAsset { get; set; }

    [JsonProperty("Impact")]
    public bool Impact { get; set; }

    [JsonProperty("Permanent")]
    public bool Permanent { get; set; }
}

public class AssetUsage
{
    [JsonProperty("Shared")]
    public bool Shared { get; set; }
}
