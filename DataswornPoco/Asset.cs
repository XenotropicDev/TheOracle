﻿using System.Collections.ObjectModel;
using Newtonsoft.Json;
namespace TheOracle2.Data;

public class Ability
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Text")]
    public string? Text { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("Alter Moves")]
    public virtual ObservableCollection<AlterMove>? AlterMoves { get; set; }

    [JsonProperty("Moves")]
    public virtual ObservableCollection<AssetMove>? Moves { get; set; }

    [JsonProperty("Alter Momentum")]
    public virtual AlterMomentum? AlterMomentum { get; set; }

    [JsonProperty("Alter Properties")]
    public virtual AlterProperties? AlterProperties { get; set; }

    [JsonProperty("Inputs")]
    public virtual ObservableCollection<Input>? Inputs { get; set; }
}

public class AlterMomentum
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Burn")]
    public virtual ObservableCollection<Burn>? Burn { get; set; }

    [JsonProperty("Reset")]
    public virtual ObservableCollection<Reset>? Reset { get; set; }
}

public class AlterMove
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Moves")]
    public ObservableCollection<string>? Moves { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger? Trigger { get; set; }

    [JsonProperty("Text")]
    public string? Text { get; set; }

    [JsonProperty("Outcomes")]
    public virtual Outcomes? Outcomes { get; set; }

    [JsonProperty("Alters")]
    public ObservableCollection<string>? Alters { get; set; }
}

public class AlterProperties
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Attachments")]
    public virtual Attachments? Attachments { get; set; }

    [JsonProperty("States")]
    public virtual ObservableCollection<State>? States { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter? ConditionMeter { get; set; }
}

public class Asset
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("Asset Type")]
    public string? AssetType { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Usage")]
    public virtual AssetUsage? Usage { get; set; }

    [JsonProperty("Attachments")]
    public virtual Attachments? Attachments { get; set; }

    [JsonProperty("Inputs")]
    public virtual ObservableCollection<Input>? Inputs { get; set; }

    [JsonProperty("Condition Meter")]
    public virtual ConditionMeter? ConditionMeter { get; set; }

    [JsonProperty("Abilities")]
    public virtual ObservableCollection<Ability>? Abilities { get; set; }

    [JsonProperty("States")]
    public virtual ObservableCollection<State>? States { get; set; }

    [JsonProperty("Requirement")]
    public string? Requirement { get; set; }

    [JsonProperty("Aliases")]
    public ObservableCollection<string>? Aliases { get; set; }

    [JsonIgnore]
    public virtual AssetRoot? Parent { get; set; }
}

public class Attachments
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Asset Types")]
    public ObservableCollection<string>? AssetTypes { get; set; }

    [JsonProperty("Max")]
    public virtual int? Max { get; set; }
}

public class Burn
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger? Trigger { get; set; }

    [JsonProperty("Effect")]
    public virtual Effect? Effect { get; set; }

    [JsonProperty("Outcomes")]
    public ObservableCollection<string>? Outcomes { get; set; }
}

public class By
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Player")]
    public bool? Player { get; set; }

    [JsonProperty("Ally")]
    public bool? Ally { get; set; }
}

public class ConditionMeter
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Min")]
    public int? Min { get; set; }

    [JsonProperty("Value")]
    public int? Value { get; set; }

    [JsonProperty("$id")]
    public string? JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Max")]
    public int? Max { get; set; }

    [JsonProperty("Conditions")]
    public ObservableCollection<string>? Conditions { get; set; }

    [JsonProperty("Aliases")]
    public ObservableCollection<string>? Aliases { get; set; }
}

public class Effect
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Text")]
    public string? Text { get; set; }
}

public class Input
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Input Type")]
    public virtual AssetInput? InputType { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Adjustable")]
    public bool? Adjustable { get; set; }

    [JsonProperty("Sets")]
    public virtual ObservableCollection<Set>? Sets { get; set; }

    [JsonProperty("Options")]
    public virtual ObservableCollection<Option>? Options { get; set; }

    [JsonProperty("Step")]
    public int? Step { get; set; }

    [JsonProperty("Min")]
    public int? Min { get; set; }

    [JsonProperty("Max")]
    public int? Max { get; set; }

    [JsonProperty("Value")]
    public int? Value { get; set; }

    [JsonProperty("Clock Type")]
    public string? ClockType { get; set; }

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
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("Category")]
    public string? Category { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Optional")]
    public bool? Optional { get; set; }

    [JsonProperty("Asset")]
    public string? Asset { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger? Trigger { get; set; }

    [JsonProperty("Text")]
    public string? Text { get; set; }

    [JsonProperty("Outcomes")]
    public virtual Outcomes? Outcomes { get; set; }

    [JsonProperty("Progress Move")]
    public bool? ProgressMove { get; set; }
}

public class Reroll
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Dice")]
    public string? Dice { get; set; }

    [JsonProperty("Text")]
    public string? Text { get; set; }
}

public class Reset
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Trigger")]
    public virtual Trigger? Trigger { get; set; }

    [JsonProperty("Value")]
    public int? Value { get; set; }
}

public class AssetRoot
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Source")]
    public virtual Source? Source { get; set; }

    [JsonProperty("$id")]
    public string JsonId { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Description")]
    public string? Description { get; set; }

    [JsonProperty("Display")]
    public virtual Display? Display { get; set; }

    [JsonProperty("Usage")]
    public virtual AssetUsage? Usage { get; set; }

    [JsonProperty("Assets")]
    public virtual ObservableCollection<Asset>? Assets { get; set; }
}

public class State
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Name")]
    public string? Name { get; set; }

    [JsonProperty("Enabled")]
    public bool? Enabled { get; set; }

    [JsonProperty("Disables asset")]
    public bool? DisablesAsset { get; set; }

    [JsonProperty("Impact")]
    public bool? Impact { get; set; }

    [JsonProperty("Permanent")]
    public bool? Permanent { get; set; }
}

public class AssetUsage
{
    [JsonIgnore]
    public uint Id { get; set; }

    [JsonProperty("Shared")]
    public bool? Shared { get; set; }
}