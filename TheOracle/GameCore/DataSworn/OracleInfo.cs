using Newtonsoft.Json;
using System.Collections.Generic;

namespace TheOracle.GameCore.Oracle.DataSworn
{
    public class AddTemplate
    {
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Template type")]
        public string TemplateType { get; set; }
    }

    public partial class Attributes
    {
        [JsonProperty(PropertyName = "Derelict Type")]
        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> DerelictType { get; set; }

        [JsonConverter(typeof(SingleOrArrayConverter<string>))]
        public List<string> Location { get; set; }
    }

    public partial class ChanceTable
    {
        [JsonProperty(PropertyName = "Add template")]
        public AddTemplate AddTemplate { get; set; }

        public List<string> Assets { get; set; }
        public int Chance { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }

        [JsonProperty(PropertyName = "Game object")]
        public GameObject GameObject { get; set; }

        [JsonProperty(PropertyName = "Multiple rolls")]
        public MultipleRolls MultipleRolls { get; set; }

        public List<Oracle> Oracles { get; set; }
        public List<Suggest> Suggest { get; set; }
        public string Thumbnail { get; set; }
        public int Value { get; set; }
    }

    public partial class GameObject
    {
        public int Amount { get; set; }
        public Attributes Attributes { get; set; }

        [JsonProperty(PropertyName = "Object type")]
        public string ObjectType { get; set; }
    }

    public class Inherit
    {
        public string Category { get; set; }
        public List<string> Exclude { get; set; }
        public List<string> Name { get; set; }
        public Requires Requires { get; set; }
    }

    public class MultipleRolls
    {
        [JsonProperty(PropertyName = "Allow duplicates")]
        public bool AllowDuplicates { get; set; }

        public int Amount { get; set; }
    }

    public partial class Oracle
    {
        public List<string> Aliases { get; set; }

        [JsonProperty(PropertyName = "Allow duplicate rolls")]
        public bool AllowDuplicateRolls { get; set; }

        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string DisplayName { get; set; }

        public bool Initial { get; set; }

        [JsonProperty(PropertyName = "Max rolls")]
        public int MaxRolls { get; set; }

        [JsonProperty(PropertyName = "Min rolls")]
        public int MinRolls { get; set; }

        public string Name { get; set; }

        [JsonProperty(PropertyName = "Oracle type")]
        public string OracleType { get; set; }

        public bool Repeatable { get; set; }
        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Select table by")]
        public string SelectTableBy { get; set; }

        public string Subgroup { get; set; }
        public List<ChanceTable> Table { get; set; }
        public List<Tables> Tables { get; set; }

        [JsonProperty(PropertyName = "Use with")]
        public List<UseWith> UseWith { get; set; }

        [JsonProperty(PropertyName = "Part of speech")]
        public List<string> PartOfSpeech { get; set; }

        [JsonProperty(PropertyName = "Content tags")]
        public List<string> ContentTags { get; set; }

        public string Group { get; set; }
    }

    public class OracleInfo
    {
        public OracleInfo()
        {
        }

        public List<string> Aliases { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string DisplayName { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public Source Source { get; set; }
        public List<Subcategory> Subcategories { get; set; }
        public List<string> Tags { get; set; }
    }

    public partial class Requires
    {
        [JsonProperty(PropertyName = "Derelict Type")]
        public List<string> DerelictType { get; set; }

        public List<string> Environment { get; set; }
        public List<string> Life { get; set; }
        public List<string> Location { get; set; }

        [JsonProperty(PropertyName = "Planetary Class")]
        public List<string> PlanetaryClass { get; set; }

        public List<string> Region { get; set; }
        public List<string> Scale { get; set; }

        [JsonProperty(PropertyName = "Starship Type")]
        public List<string> StarshipType { get; set; }

        [JsonProperty(PropertyName = "Theme Type")]
        public List<string> ThemeType { get; set; }

        public List<string> Type { get; set; }

        public List<string> Zone { get; set; }
    }

    public class Subcategory
    {
        public List<string> Aliases { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string Displayname { get; set; }

        public List<Inherit> Inherits { get; set; }
        public string Name { get; set; }
        public List<Oracle> Oracles { get; set; }
        public Requires Requires { get; set; }

        [JsonProperty(PropertyName = "Sample Names")]
        public List<string> SampleNames { get; set; }

        public Source Source { get; set; }
        public string Thumbnail { get; set; }
    }

    public class Suggest
    {
        [JsonProperty(PropertyName = "Game object")]
        public GameObject GameObject { get; set; }

        public List<Oracle> Oracles { get; set; }
    }

    public class Tables
    {
        public List<string> Aliases { get; set; }

        [JsonProperty(PropertyName = "Display name")]
        public string DisplayName { get; set; }

        public string Name { get; set; }
        public Requires Requires { get; set; }
        public List<ChanceTable> Table { get; set; }
    }

    public class UseWith
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
    }
}