﻿// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dataforged
{
    public class RegionEntry
    {
        [JsonPropertyName("description")]
        public MarkdownString Description { get; set; }

        [JsonPropertyName("features")]
        public IList<MarkdownString> Features { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("quest_starter")]
        public MarkdownString QuestStarter { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; }

        [JsonPropertyName("summary")]
        public MarkdownString Summary { get; set; }

        [JsonPropertyName("suggestions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Suggestions? Suggestions { get; set; }
    }
}
