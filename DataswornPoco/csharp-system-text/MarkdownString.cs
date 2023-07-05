﻿// Code generated by jtd-codegen for C# + System.Text.Json v0.2.1

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dataforged
{
    /// <summary>
    /// A rich text string in Markdown. Usually this is a direct excerpt from
    /// the rules text.
    /// 
    ///       The custom syntax `{{table:some_oracle_table_id}}` represents a
    /// markdown table rendered from oracle data.
    /// </summary>
    [JsonConverter(typeof(MarkdownStringJsonConverter))]
    public class MarkdownString
    {
        /// <summary>
        /// The underlying data being wrapped.
        /// </summary>
        public string Value { get; set; }

        public static implicit operator MarkdownString(string s) => new() { Value = s };
    }

    public class MarkdownStringJsonConverter : JsonConverter<MarkdownString>
    {
        public override MarkdownString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new MarkdownString { Value = JsonSerializer.Deserialize<string>(ref reader, options) };
        }

        public override void Write(Utf8JsonWriter writer, MarkdownString value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize<string>(writer, value.Value, options);
        }
    }
}