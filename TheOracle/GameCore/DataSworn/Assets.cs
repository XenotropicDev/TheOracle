using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TheOracle.GameCore.Oracle.DataSworn
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Ability
    {
        [Newtonsoft.Json.JsonProperty("Alter Moves", Required = Newtonsoft.Json.Required.Default)]
        public IList<AlterMove> AlterMoves { get; set; }

        [Newtonsoft.Json.JsonProperty("Alter Properties", Required = Newtonsoft.Json.Required.Default)]
        public AlterProperties AlterProperties { get; set; }

        [Newtonsoft.Json.JsonProperty("Counter")]
        public Counter Counter { get; set; }

        [Newtonsoft.Json.JsonProperty("Enabled")]
        public bool Enabled { get; set; }
        
        [Newtonsoft.Json.JsonProperty("Input")]
        public System.Collections.Generic.IList<string> Input { get; set; }

        public Move Move { get; set; }

        [Newtonsoft.Json.JsonProperty("Text")]
        public string Text { get; set; }

        public Ability DeepCopy()
        {
            Ability clone = (Ability)this.MemberwiseClone();

            clone.AlterMoves = new List<AlterMove>(this.AlterMoves?.Select(am => am.DeepCopy()) ?? new List<AlterMove>());
            clone.AlterProperties = this.AlterProperties?.DeepCopy();
            clone.Counter = this.Counter?.DeepCopy();
            clone.Input = new List<string>(this.Input ?? new List<string>());
            clone.Move = this.Move?.DeepCopy();

            return clone;
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class AlterProperties
    {
        [Newtonsoft.Json.JsonProperty("Condition Meter")]
        public ConditionMeter ConditionMeter { get; set; }
        
        [Newtonsoft.Json.JsonProperty("Track")]
        public Track Track { get; set; }

        internal AlterProperties DeepCopy()
        {
            var clone = (AlterProperties)this.MemberwiseClone();
            clone.ConditionMeter = this.ConditionMeter?.DeepCopy();
            clone.Track = this.Track?.DeepCopy();

            return clone;
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Asset
    {
        private string category;

        [Newtonsoft.Json.JsonProperty("Abilities")]
        public System.Collections.Generic.IList<Ability> Abilities { get; set; }

        [Newtonsoft.Json.JsonProperty("Aliases")]
        public System.Collections.Generic.IList<string> Aliases { get; set; }

        [Newtonsoft.Json.JsonProperty("Asset Type")]
        public string AssetType { get => category; set => category = value; }

        [Newtonsoft.Json.JsonProperty("Category")]
        public string Category { get => category; set => category = value; }

        [Newtonsoft.Json.JsonProperty("Condition Meter")]
        public ConditionMeter ConditionMeter { get; set; }

        [Newtonsoft.Json.JsonProperty("Counter")]
        public Counter Counter { get; set; }

        [Newtonsoft.Json.JsonProperty("Description")]
        public string Description { get; set; }

        [Newtonsoft.Json.JsonProperty("Input")]
        [NotMapped]
        public System.Collections.Generic.IList<string> Input { get; set; }

        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Select")]
        public Select Select { get; set; }

        [Newtonsoft.Json.JsonProperty("Track")]
        public Track Track { get; set; }
    }

    public class ConditionMeter
    {
        public string Name { get; set; }
        public int Max { get; set; }

        [Newtonsoft.Json.JsonProperty("Starts At")]
        public int StartsAt { get; set; }

        internal ConditionMeter DeepCopy()
        {
            return (ConditionMeter)this.MemberwiseClone();
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Counter
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Starts At")]
        public int StartsAt { get; set; }

        [Newtonsoft.Json.JsonProperty("Max")]
        public int Max { get; set; }

        internal Counter DeepCopy()
        {
            return (Counter)this.MemberwiseClone();
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Source : ISourceInfo
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Page")]
        public string Page { get; set; }

        [Newtonsoft.Json.JsonProperty("Date", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Date { get; set; }

    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class Track
    {
        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Starts At", Required = Newtonsoft.Json.Required.Always)]
        public int StartsAt { get; set; }

        [Newtonsoft.Json.JsonProperty("Value", Required = Newtonsoft.Json.Required.Always)]
        public int Value { get; set; }

        internal Track DeepCopy()
        {
            return (Track)this.MemberwiseClone();
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.5.2.0 (Newtonsoft.Json v12.0.0.0)")]
    public class AssetRoot
    {
        [Newtonsoft.Json.JsonProperty("Assets")]
        public System.Collections.Generic.IList<Asset> Assets { get; set; }

        [Newtonsoft.Json.JsonProperty("Name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("Source")]
        public Source Source { get; set; }

        //Todo Get rsek to convert these to tags with IDs?
        [Newtonsoft.Json.JsonProperty("Tags")]
        public System.Collections.Generic.IList<string> Tags { get; set; }

    }

    public class AlterMove
    {
        [Newtonsoft.Json.JsonProperty("Any Move")]
        public bool AnyMove { get; set; }
        public string Name { get; set; }
        public IList<Trigger> Triggers { get; set; }

        internal AlterMove DeepCopy()
        {
            AlterMove clone = (AlterMove)this.MemberwiseClone();
            clone.Triggers = new List<Trigger>(this.Triggers?.Select(t => t.DeepCopy()) ?? new List<Trigger>());
            
            return clone;
        }
    }


    public class Select
    {
        [Newtonsoft.Json.JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IList<string> Options { get; set; }
    }

}