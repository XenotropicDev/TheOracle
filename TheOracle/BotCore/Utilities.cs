using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace TheOracle.BotCore
{
    public static class Utilities
    {
        public static decimal ConvertPercentToDecimal(string percentValue, CultureInfo culture = default)
        {
            if (culture == default) culture = CultureInfo.CurrentCulture;

            var numFormat = culture.NumberFormat;

            NumberFormatInfo nfi = new NumberFormatInfo()
            {
                CurrencyDecimalDigits = numFormat.PercentDecimalDigits,
                CurrencyDecimalSeparator = numFormat.PercentDecimalSeparator,
                CurrencyGroupSeparator = numFormat.PercentGroupSeparator,
                CurrencyGroupSizes = numFormat.PercentGroupSizes,
                CurrencyNegativePattern = numFormat.PercentNegativePattern,
                CurrencyPositivePattern = numFormat.PercentPositivePattern,
                CurrencySymbol = numFormat.PercentSymbol
            };

            var convertedValue = decimal.Parse(percentValue, NumberStyles.Currency, nfi);
            return convertedValue / 100m;
        }

        public static GameName GetGameContainedInString(string value)
        {
            if (value.Length == 0) return GameName.None;

            foreach (var s in Enum.GetNames(typeof(GameName)).Where(g => !g.Equals("none", StringComparison.OrdinalIgnoreCase)))
            {
                if (Regex.IsMatch(value, s, RegexOptions.IgnoreCase) && Enum.TryParse(s, out GameName game))
                {
                    return game;
                }
            }
            return GameName.None;
        }

        public static string RemoveGameNamesFromString(string value)
        {
            if (value.Length == 0) return value;

            foreach (var s in Enum.GetNames(typeof(GameName)).Where(g => !g.Equals("none", StringComparison.OrdinalIgnoreCase)))
            {
                value = Regex.Replace(value, $"{s} ?", "", RegexOptions.IgnoreCase).Trim();
            }
            return value;
        }

        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool UndoFormatString(this string data, string format, out string[] values, bool ignoreCase)
        {
            int tokenCount = 0;
            format = Regex.Escape(format).Replace("\\{", "{");

            for (tokenCount = 0; ; tokenCount++)
            {
                string token = string.Format("{{{0}}}", tokenCount);
                if (!format.Contains(token)) break;
                format = format.Replace(token,
                    string.Format("(?'group{0}'.*)", tokenCount));
            }

            RegexOptions options =
                ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            Match match = new Regex(format, options).Match(data);

            if (tokenCount != (match.Groups.Count - 1))
            {
                values = new string[] { };
                return false;
            }
            else
            {
                values = new string[tokenCount];
                for (int index = 0; index < tokenCount; index++)
                    values[index] =
                        match.Groups[string.Format("group{0}", index)].Value;
                return true;
            }
        }
    }
}