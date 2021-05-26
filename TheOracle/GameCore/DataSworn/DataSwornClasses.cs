using Newtonsoft.Json;
using System.Collections.Generic;

namespace TheOracle.GameCore.Oracle.DataSworn
{
    public class Ability
    {
        [JsonProperty(PropertyName = "Alter Properties")]
        public AlterProperties AlterProperties { get; set; }

        public bool Enabled { get; set; }
        public string[] Fields { get; set; }
        public string Text { get; set; }
    }

    public class AlterProperties
    {
        public Track Track { get; set; }
    }

    public class Asset
    {
        public List<Ability> Abilities { get; set; }
        public string Category { get; set; }
        public Counter Counter { get; set; }
        public string Description { get; set; }
        public string[] Fields { get; set; }
        public string Name { get; set; }
        public Track Track { get; set; }
    }

    public class AssetInfo
    {
        public List<Asset> Assets { get; set; }
        public string Name { get; set; }
        public Source Source { get; set; }
        public string[] Tags { get; set; }
    }

    public class Counter
    {
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Starts At")]
        public int StartsAt { get; set; }
    }

    public class GlossaryInfo
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public Source Source { get; set; }
        public List<GlossaryTerm> Terms { get; set; }
    }

    public class GlossaryTerm
    {
        public string Color { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }

    public class Inherit
    {
        public string From { get; set; }
        public string[] Oracles { get; set; }
        public Requires Requires { get; set; }
    }

    public class Move
    {
        public string Category { get; set; }
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Progress Move")]
        public bool ProgressMove { get; set; }

        public string Text { get; set; }
    }

    public class MoveInfo
    {
        public Move[] Moves { get; set; }
        public string Name { get; set; }
        public Source Source { get; set; }
        public string[] Tags { get; set; }
    }

    public class Oracle
    {
        private string description;

        public string[] Aliases { get; set; }

        public string Character { get; set; }

        public string Description { get => (description == "[Roll three times]") ? "[3x]" : description; set => description = value; }

        public bool Initial { get; set; }

        [JsonProperty(PropertyName = "Ironsworn PlaceHolder")]
        public bool IronswornPlaceholder { get; set; }

        [JsonProperty(PropertyName = "Max rolls")]
        public int Maxrolls { get; set; }

        [JsonProperty(PropertyName = "Min rolls")]
        public int Minrolls { get; set; }

        public string Name { get; set; }
        public Requires Requires { get; set; }
        public Source Source { get; set; }
        public List<Table> Table { get; set; }
        public List<Tables> Tables { get; set; }
        public string[] Tags { get; set; }
    }

    public class OracleInfo
    {
        public string[] Aliases { get; set; }

        public string Category { get; set; }

        public string[] Children { get; set; }

        public string Description { get; set; }

        public List<Inherit> Inherits { get; set; }

        public string Name { get; set; }

        public List<Oracle> Oracles { get; set; }

        public string Parent { get; set; }

        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Sample Names")]
        public string[] SampleNames { get; set; }

        public Source Source { get; set; }
        public string[] Tags { get; set; }
        public string Thumbnail { get; set; }
        public string Type { get; set; }
    }

    public class Requires
    {
        [JsonProperty(PropertyName = "Derelict Type")]
        public string[] DerelictType { get; set; }

        public string[] Environment { get; set; }
        public string[] Life { get; set; }

        public string[] Location { get; set; }

        [JsonProperty(PropertyName = "Planetary Class")]
        public string[] PlanetaryClass { get; set; }

        public string[] Region { get; set; }
        public string[] Scale { get; set; }

        [JsonProperty(PropertyName = "Starship Type")]
        public string[] StarshipType { get; set; }

        [JsonProperty(PropertyName = "Theme Type")]
        public string[] ThemeType { get; set; }

        public string[] Type { get; set; }
        public string[] Zone { get; set; }

        //Todo
        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class Source
    {
        public string Name { get; set; }
        public string Page { get; set; }
        public string Version { get; set; }
    }

    public class Table
    {
        private string description;

        public int Chance { get; set; }

        [JsonProperty(PropertyName = "Table")]
        public List<Table> ChildTable { get; set; }

        public string Description { get => (description == "[Roll three times]") ? "[3x]" : description; set => description = value; }

        public int Value { get; set; }
        public string Details { get; set; }

        [JsonProperty(PropertyName = "Quest Starter")]
        public string QuestStarter { get; set; }

        public string Thumbnail { get; set; }
    }

    public class Tables
    {
        private string description;

        public string[] Aliases { get; set; }
        public int Chance { get; set; }
        public string Description { get => (description == "[Roll three times]") ? "[3x]" : description; set => description = value; }

        public bool Initial { get; set; }
        public Requires Requires { get; set; }
        public List<Table> Table { get; set; }
        public string[] Tags { get; set; }
        public string Thumbnail { get; set; }
    }

    public class Track
    {
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Starts At")]
        public int StartsAt { get; set; }

        public int Value { get; set; }
    }
}