using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tameenk.Common.Utilities
{
    public  class CacheUtility
    {
        private readonly IMemoryCache _memoryCache;

        public CacheUtility(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public object GetValueFromCache(string CacheKey)
        {
            _memoryCache.TryGetValue(CacheKey, out var result);
            return result;
        }

        public void AddValueToCache(string CacheKey, object obj)
        {
            lock (_memoryCache)
            {
                _memoryCache.Set(CacheKey, obj, DateAndTime.Now.AddMinutes(1));
            }
        }

        public void AddValueToCache(string CacheKey, object obj, int Minutes)
        {
            lock (_memoryCache)
            {
                _memoryCache.Set(CacheKey, obj, DateAndTime.Now.AddMinutes(Minutes));
            }
        }

        public void RemoveCache(string CacheKey)
        {
            _memoryCache.Remove(CacheKey);
        }

        public void RemoveAllCacheKey()
        {
            // Get all keys from the cache
            var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic);
            var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(_memoryCache) as dynamic;
            var cacheCollectionValues = new List<ICacheEntry>();
            foreach (var cacheItem in cacheEntriesCollection)
            {
                ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);
                cacheCollectionValues.Add(cacheItemValue);
            }

            // Remove all items from cache
            foreach (var cacheItem in cacheCollectionValues)
            {
                _memoryCache.Remove(cacheItem.Key);
            }
        }
    }
}
