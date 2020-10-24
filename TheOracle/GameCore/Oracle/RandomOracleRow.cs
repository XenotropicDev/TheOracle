using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TheOracle.StarForged;

namespace TheOracle.Core
{
    public static class RandomOracleRow
    {
        public static T GetRandomRow<T>(this IEnumerable<T> source, Random random = default, int dieSize = 100) where T : IOracleEntry
        {
            if (source.Count() == 0) return default;
            if (random == default) random = BotRandom.Instance;
            int roll = random.Next(1, dieSize);
            return source.OrderBy(item => item.Chance).FirstOrDefault(item => item.Chance >= roll);
        }

        public static T LookupOracle<T>(this IEnumerable<T> source, int roll) where T : IOracleEntry
        {
            if (source.Count() == 0) return default;
            return source.OrderBy(item => item.Chance).FirstOrDefault(item => item.Chance >= roll);
        }

        public static void Shuffle<T>(this IList<T> list, Random random = default)
        {
            if (random == default) random = BotRandom.Instance;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
