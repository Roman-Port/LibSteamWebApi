using LibSteamWebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibSteamWebApi.Support
{
    public abstract class BlockRequest<TKey, TValue>
    {
        internal BlockRequest()
        {
        }

        private readonly Dictionary<TKey, TaskCompletionSource<TValue>> items = new Dictionary<TKey, TaskCompletionSource<TValue>>();

        protected abstract TKey GetKeyFromValue(TValue value);
        protected abstract Task<IList<TValue>> FetchResults(ICollection<TKey> value);

        public BlockRequest<TKey, TValue> ScheduleFetch(params TKey[] ids)
        {
            foreach (var id in ids)
            {
                if (!items.ContainsKey(id))
                    items.Add(id, new TaskCompletionSource<TValue>());
            }
            return this;
        }

        public Task<TValue> ScheduleFetchGetResultAsync(TKey id)
        {
            ScheduleFetch(id);
            return items[id].Task;
        }

        public async Task<BlockResult<TKey, TValue>> Submit()
        {
            //Fetch
            IList<TValue> raw;
            try
            {
                raw = await FetchResults(items.Keys);
            }
            catch (Exception ex)
            {
                throw new SteamRequestErrorException(ex);
            }

            //Loop through and send completions
            foreach (var p in raw)
                items[GetKeyFromValue(p)].SetResult(p);

            //Signal all unsignaled (and thus not found) items
            foreach (var i in items)
            {
                if (!i.Value.Task.IsCompleted)
                    i.Value.SetException(new SteamNotFoundException());
            }

            return new BlockResult<TKey, TValue>(items);
        }
    }
}
