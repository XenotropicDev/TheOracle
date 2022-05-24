using System;
using System.Collections.Generic;
using System.Linq;

namespace TheOracle.GameCore.Oracle.DataSworn
{
    public class Move
    {
        public string Asset { get; set; }
        public string Category { get; set; }
        public string Name { get; set; }
        public bool Oracle { get; set; }

        [Newtonsoft.Json.JsonProperty("Progress Move")]
        public bool IsProgressMove { get; set; }

        public string Text { get; set; }
        public List<Trigger> Triggers { get; set; }

        internal Move DeepCopy()
        {
            var clone = (Move)MemberwiseClone();

            clone.Triggers = new List<Trigger>(this.Triggers?.Select(t => t.DeepCopy()) ?? new List<Trigger>());

            return clone;
        }
    }

    public class MovesInfo
    {
        public List<Move> Moves { get; set; }
        public string Name { get; set; }
        public Source Source { get; set; }
    }

    public class StatOptions
    {
        public string Method { get; set; }
        public IList<string> Stats { get; set; }
        public IList<string> Resources { get; set; }

        internal StatOptions DeepCopy()
        {
            StatOptions clone = (StatOptions)this.MemberwiseClone();
            clone.Stats = new List<string>(this.Stats ?? new List<string>());
            clone.Resources = new List<string>(this.Resources ?? new List<string>());

            return clone;
        }
    }

    public class Trigger
    {
        public string Details { get; set; }

        [Newtonsoft.Json.JsonProperty("Stat Options")]
        public StatOptions StatOptions { get; set; }

        public string Text { get; set; }

        internal Trigger DeepCopy()
        {
            var clone = (Trigger)this.MemberwiseClone();
            clone.StatOptions = this.StatOptions.DeepCopy();
            
            return clone;
        }
    }
}