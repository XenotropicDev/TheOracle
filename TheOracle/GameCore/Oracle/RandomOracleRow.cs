using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using TheOracle.BotCore;

namespace TheOracle.Core
{
    public static class RandomOracleRow
    {
        public static T GetRandomRow<T>(this IEnumerable<T> source, Random random = default, int dieSize = 100) where T : IOracleEntry
        {
            if (source.Count() == 0) return default;
            if (random == default) random = BotRandom.Instance;
            int roll = random.Next(1, dieSize + 1);
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

        /// <summary>
        /// Adds an oracle description value to the list, observing the channel settings for duplicate rolls.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="table">The Oracle table to roll</param>
        /// <param name="game">The game of the oracle table</param>
        /// <param name="services">The DI service container</param>
        /// <param name="channelId">The channel the request originated from</param>
        /// <param name="random">The random instance to use</param>
        public static void AddRandomOracleRow(this IList<string> source, string table, GameName game, IServiceProvider services, ulong channelId = 0, Random random = default)
        {
            if (random == default) random = BotRandom.Instance;
            OracleService oracles = services.GetRequiredService<OracleService>();
            bool retry = true;

            while (retry)
            {
                string result = oracles.RandomOracleResult(table, services, game, random);
                if (source.Contains(result) && channelId > 0)
                {
                    ChannelSettings channelSettings = ChannelSettings.GetChannelSettingsAsync(channelId).Result;
                    if (channelSettings.RerollDuplicates) continue;
                }
                retry = false;
                source.Add(result);
            }
        }
    }
}