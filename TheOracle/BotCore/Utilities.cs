using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TheOracle.GameCore;

namespace TheOracle.BotCore
{
    public static class Utilities
    {
        public static async Task<SocketMessage> NextChannelMessageAsync(this IChannel channel,
            DiscordSocketClient client,
            IUser user = null,
            TimeSpan? timeout = null,
            CancellationToken token = default(CancellationToken))
        {
            timeout = timeout ?? TimeSpan.FromSeconds(30);

            var eventTrigger = new TaskCompletionSource<SocketMessage>();
            var cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true));

            async Task Handler(SocketMessage message)
            {
                var result = message.Channel.Id == channel.Id && (message.Author.Id == user.Id || user == null);
                if (result) eventTrigger.SetResult(message);
            }

            client.MessageReceived += Handler;

            var trigger = eventTrigger.Task;
            var cancel = cancelTrigger.Task;
            var delay = Task.Delay(timeout.Value);
            var task = await Task.WhenAny(trigger, delay, cancel).ConfigureAwait(false);

            client.MessageReceived -= Handler;

            if (task == trigger)
                return await trigger.ConfigureAwait(false);
            else
                return null;
        }

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

        public static bool Contains(this IEmote source, IEmote[] emotesToCheck)
        {
            return emotesToCheck.Any(check => check.Name == source.Name);
        }

        public static bool Contains(this IEmote source, IEmote emoteToCheck)
        {
            return source.Name == emoteToCheck.Name;
        }

        public static bool IsSameAs(this IEmote source, IEmote emoteToCheck)
        {
            if (source.Name == emoteToCheck.Name) return true;

            List<Tuple<IEmote, IEmote>> EmotesThatAreTheSame = new List<Tuple<IEmote, IEmote>>
            {
                new Tuple<IEmote, IEmote>(new Emoji("0️⃣"), new Emoji("\u0030\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("1️⃣"), new Emoji("\u0031\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("2️⃣"), new Emoji("\u0032\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("3️⃣"), new Emoji("\u0033\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("4️⃣"), new Emoji("\u0034\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("5️⃣"), new Emoji("\u0035\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("6️⃣"), new Emoji("\u0036\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("7️⃣"), new Emoji("\u0037\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("8️⃣"), new Emoji("\u0038\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("9️⃣"), new Emoji("\u0039\u20E3")),
                new Tuple<IEmote, IEmote>(new Emoji("\\⏺️"), new Emoji("⏺️")),
            };

            foreach (var tuple in EmotesThatAreTheSame)
            {
                if ((source.Contains(tuple.Item1) || source.Contains(tuple.Item2)) && (emoteToCheck.Contains(tuple.Item1) || emoteToCheck.Contains(tuple.Item2))) return true;
            }
            return false;
        }

        public static bool IsSameAs(this IEmote source, string emoteToCheck)
        {
            return source.IsSameAs(new Emoji(emoteToCheck));
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

        public static string ReplaceFirst(this string text, string search, string replace, StringComparison comparer = StringComparison.Ordinal)
        {
            int pos = text.IndexOf(search, comparer);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static bool UndoFormatString(this string data, string format, out string[] values, bool ignoreCase = true)
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
                values = null;
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