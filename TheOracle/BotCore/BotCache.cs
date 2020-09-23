using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheOracle.Core
{
    public static class BotCache
    {
        private static MemoryCache cache = default;
        public static MemoryCache Get()
        {
            if (cache == default)
            {
                var cacheOptions = new MemoryCacheOptions();
                cache = new MemoryCache(cacheOptions);
            }

            return cache;
        }
    }
}
