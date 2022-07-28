using LibSteamWebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibSteamWebApi.Support
{
    public class BlockResult<TKey, TValue>
    {
        public BlockResult(Dictionary<TKey, TaskCompletionSource<TValue>> items)
        {
            this.items = items;
        }

        private readonly Dictionary<TKey, TaskCompletionSource<TValue>> items;

        internal void CopyTo(IList<TValue> output)
        {
            foreach (var i in items)
            {
                Task<TValue> task = i.Value.Task;
                if (task.IsCompletedSuccessfully)
                    output.Add(task.Result);
            }
        }

        public TValue Get(TKey id)
        {
            if (items.TryGetValue(id, out TaskCompletionSource<TValue> item))
                return item.Task.Result;
            else
                throw new SteamNotRequestedException();
        }

        public bool TryGet(TKey id, out TValue result)
        {
            if (items.TryGetValue(id, out TaskCompletionSource<TValue> item) && item.Task.IsCompletedSuccessfully)
            {
                result = item.Task.Result;
                return true;
            } else
            {
                result = default(TValue);
                return false;
            }
        }
    }
}
