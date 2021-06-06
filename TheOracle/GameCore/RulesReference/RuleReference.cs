using Discord;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TheOracle.BotCore;

namespace TheOracle.GameCore.RulesReference
{
    public class Move
    {
        public string[] Aliases { get; set; }
        public GameName Game { get; set; }
        public bool IsProgressMove { get; set; }
        public string Name { get; set; }
        public SourceInfo Source { get; set; }
        public string Text { get; set; }

        public List<Embed> AsEmbed(RuleReference rules = null)
        {
            var embedList = new List<Embed>();

            var builder = new EmbedBuilder();
            builder.Title = Name;
            string moveText = (IsProgressMove) ? $"_{RulesResources.ProgressMove}_\n\n{Text}" : Text;

            if (moveText.Length > EmbedBuilder.MaxDescriptionLength)
            {
                int lastCutoff = 0;
                while (lastCutoff >= 0)
                {
                    int endOfCheck = Math.Min(EmbedFieldBuilder.MaxFieldValueLength, moveText.Length - lastCutoff);
                    if (lastCutoff == 0) endOfCheck = EmbedBuilder.MaxDescriptionLength;

                    int endOfField = moveText.Substring(0, lastCutoff + endOfCheck).LastIndexOf("\n");

                    string fieldText = (endOfField - lastCutoff > 0 && endOfField != lastCutoff) ? moveText.Substring(lastCutoff, endOfField - lastCutoff) : moveText.Substring(lastCutoff);
                    bool seperateEmbed = false;

                    int codeBlockDelimiters = fieldText.CountOccurrences("```");
                    if (codeBlockDelimiters > 0)
                    {
                        int startOfBlock = moveText.Substring(0, lastCutoff + endOfCheck).IndexOf("```");
                        int endOfBlock = moveText.IndexOf("```", startOfBlock + 3) + 3;

                        if (fieldText.StartsWith("```"))
                        {
                            fieldText = moveText.Substring(startOfBlock, endOfBlock - startOfBlock);
                            endOfField = endOfBlock;

                            if (!Regex.IsMatch(moveText, @"\n(\|[^|\n]*){4,}")) //If it has more than 2 columns just use a code block and pray it looks good.
                            {
                                fieldText = FormatTable(fieldText);
                            }

                            if (fieldText.Length > EmbedFieldBuilder.MaxFieldValueLength)
                            {
                                embedList.Add(new EmbedBuilder().WithDescription(fieldText).Build());
                                seperateEmbed = true;
                            }
                        }
                        else
                        {
                            fieldText = fieldText.Substring(0, fieldText.IndexOf("```"));
                            endOfField = moveText.Substring(0, lastCutoff + endOfCheck).IndexOf("```");
                        }
                    }

                    if (lastCutoff == 0) builder.WithDescription(fieldText);
                    else if (!seperateEmbed && !string.IsNullOrEmpty(fieldText)) builder.AddField("-", fieldText);

                    lastCutoff = (endOfField != lastCutoff) ? endOfField : -1;
                    if (lastCutoff == moveText.Length) lastCutoff = -1;
                }
            }
            else
            {
                if (moveText.Contains("```") && !Regex.IsMatch(moveText, @"\n(\|[^|\n]*){4,}")) //If it has more than 2 columns just use a code block and pray it looks good.
                {
                    moveText = FormatTable(moveText);
                }

                builder.WithDescription(moveText);
            }

            builder.WithFooter(Source.ToString());

            if (rules != null)
            {
                builder.WithAuthor(rules.Category);
            }

            embedList.Insert(0, builder.Build());
            return embedList;
        }

        private static string FormatTable(string tableText)
        {
            tableText = Regex.Replace(tableText, @"\n +", " ").Replace(" +", " ");
            tableText = Regex.Replace(tableText, @" +\|?(\n|$)", "$1");
            tableText = Regex.Replace(tableText, "(\\|?---+\\|?)+", "");
            tableText = Regex.Replace(tableText, @"\n\| ?([^|]*)\|", "\n`$1`");
            tableText = tableText.Replace("```", "").Replace("|", "");
            return tableText;
        }
    }

    public class RuleReference
    {
        public string[] Aliases { get; set; }
        public string Category { get; set; }
        public GameName Game { get; set; }
        public List<Move> Moves { get; set; }
        public SourceInfo Source { get; set; }
    }
}