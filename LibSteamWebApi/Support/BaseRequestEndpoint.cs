using LibSteamWebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibSteamWebApi.Support
{
    public abstract class BaseRequestEndpoint<TKey, TValue>
    {
        public abstract BlockRequest<TKey, TValue> CreateRequestBuilder();
        public abstract TKey GetKeyFromValue(TValue value);

        public async Task<TValue> FetchOne(TKey id)
        {
            var result = await CreateRequestBuilder()
                .ScheduleFetch(id)
                .Submit();
            return result.Get(id);
        }

        public Task<BlockResult<TKey, TValue>> FetchMultiple(IEnumerable<TKey> id)
        {
            return CreateRequestBuilder()
                .ScheduleFetch(id.ToArray())
                .Submit();
        }

        public Task<BlockResult<TKey, TValue>> FetchMultiple<TInternal>(IEnumerable<TInternal> items, Func<TInternal, TKey> each)
        {
            List<TKey> ids = new List<TKey>();
            foreach (var i in items)
                ids.Add(each(i));
            return FetchMultiple(ids);
        }

        public Task<BlockResult<TKey, TValue>> FetchMultiple(params TKey[] ids)
        {
            return FetchMultiple((IEnumerable<TKey>)ids);
        }
    }
}
