using LibSteamWebApi.Support;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibSteamWebApi
{
    public class SteamCacheWrapper<TKey, TValue> : BaseProxyWrapper<TKey, TValue>
    {
        public SteamCacheWrapper(BaseRequestEndpoint<TKey, TValue> baseEndpoint, TimeSpan cacheLifetime) : base(baseEndpoint)
        {
            this.cacheLifetime = cacheLifetime;
        }

        private readonly TimeSpan cacheLifetime;
        private readonly Dictionary<TKey, CacheItem> cache = new Dictionary<TKey, CacheItem>();

        protected override bool TryGetFromCache(TKey key, out TValue value)
        {
            if (cache.TryGetValue(key, out CacheItem cacheValue) && cacheValue.Age < cacheLifetime)
            {
                value = cacheValue.Item;
                return true;
            } else
            {
                value = default(TValue);
                return false;
            }
        }

        protected override void InsertToCache(ICollection<TValue> results)
        {
            foreach (var r in results)
            {
                TKey key = GetKeyFromValue(r);
                if (cache.TryGetValue(key, out CacheItem item))
                    item.Update(r);
                else
                    cache.Add(key, new CacheItem(r));
            }
        }

        class CacheItem
        {
            public CacheItem(TValue item)
            {
                Update(item);
            }

            private TValue item;
            private DateTime time;

            public TimeSpan Age => DateTime.UtcNow - time;
            public TValue Item => item;

            public void Update(TValue item)
            {
                this.item = item;
                time = DateTime.UtcNow;
            }
        }
    }
}
