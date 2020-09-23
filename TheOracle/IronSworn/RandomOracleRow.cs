using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOracle.Core
{
    public static class RandomOracleRow
    {
        public static T GetRandomRow<T>(this IEnumerable<T> source, Random random = default) where T : IOracleChance
        {
            if (source.Count() == 0) return default;
            if (random == default) random = BotRandom.Instance;
            return source.OrderBy(item => item.Chance).First(item => item.Chance >= random.Next(1, 100));
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
