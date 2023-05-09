using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Server.DiscordServer
{
    public class DiscordHelpers
    {
        public static string FormatMarkdownLinks(string text, bool switchToUnderline = true)
        {
            var markDownRegex = new Regex(@"\[(.*?)\](\(.*?\))");

            var match = markDownRegex.Match(text);
            if (!match.Success) {  return text; }

            var replacement = switchToUnderline ? "__$1__" : "$1";

            text = Regex.Replace(text, markDownRegex.ToString(), replacement);

            return text;
        }
    }
}
