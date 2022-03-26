using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TheOracle.BotCore;
using TheOracle.GameCore.Oracle.DataSworn;

namespace TheOracle.GameCore.DataSworn
{
    class MoveAdapter : RulesReference.Move
    {
        public MoveAdapter(Move move, Source source, GameName game)
        {
            this.Name = move.Name;
            this.Game = game;

            string text = Utilities.FormatMarkdown(move.Text);
            if (Regex.IsMatch(text, @"\| ?:?---")) //markdown table
            {
                /* repair point (up to 3 points).\n\n
                 *
                 * | Situation     | Strong Hit | Weak Hit |\n
                 * |---------------|------------|----------|\n
                 * | At a facility | 5 points   | 3 points |\n
                 * | In the field  | 3 points   | 1 points |\n
                 * | Under fire    | 2 points   | 0 points |\n\n
                 *
                 * Spend repair points as follows.
                 */

                var tables = Regex.Matches(text, @"(\| ?:?---+:? ?)+");
                foreach (Match table in tables)
                {
                    int startOfHeaders = text.Substring(0, Math.Max(table.Index - 2, 1)).LastIndexOf("\n") + 1;
                    int endOfTable = text.IndexOf("\n\n", startOfHeaders);

                    string fullTable = (endOfTable - startOfHeaders > 0) ? text.Substring(startOfHeaders, endOfTable - startOfHeaders) : text.Substring(startOfHeaders);
                    text = text.Replace(fullTable, $"```{fullTable}```");
                }

            }

            this.Text = text;

            this.IsProgressMove = move.IsProgressMove;

            this.Source = new SourceAdapter(source);
        }
    }
}
