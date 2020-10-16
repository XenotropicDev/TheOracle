using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TheOracle.IronSworn;

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

        internal static GameName GetDefaultGame()
        {
            return GameName.Ironsworn;
        }

        public static GameName GetGameContainedInString(string value)
        {
            if (value.Length == 0) return GameName.None;

            foreach (var s in Enum.GetNames(typeof(GameName)).Where(g => !g.Equals("none", StringComparison.OrdinalIgnoreCase)))
            {
                if (Regex.IsMatch(value, $"(^{s} | {s}( |$)|^{s}$)", RegexOptions.IgnoreCase) && Enum.TryParse(s, out GameName game))
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
    }
}
