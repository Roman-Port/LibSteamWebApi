using LibSteamWebApi.Support;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibSteamWebApi.Support
{
    public abstract class BaseProxyWrapper<TKey, TValue> : BaseRequestEndpoint<TKey, TValue>
    {
        public BaseProxyWrapper(BaseRequestEndpoint<TKey, TValue> baseEndpoint)
        {
            this.baseEndpoint = baseEndpoint;
        }

        private BaseRequestEndpoint<TKey, TValue> baseEndpoint;

        public override BlockRequest<TKey, TValue> CreateRequestBuilder()
        {
            return new ProxyRequest(this);
        }

        public override TKey GetKeyFromValue(TValue value)
        {
            return baseEndpoint.GetKeyFromValue(value);
        }

        protected abstract bool TryGetFromCache(TKey key, out TValue value);
        protected abstract void InsertToCache(ICollection<TValue> results);

        class ProxyRequest : BlockRequest<TKey, TValue>
        {
            internal ProxyRequest(BaseProxyWrapper<TKey, TValue> endpoint)
            {
                this.endpoint = endpoint;
            }

            private readonly BaseProxyWrapper<TKey, TValue> endpoint;

            protected override TKey GetKeyFromValue(TValue value)
            {
                return endpoint.GetKeyFromValue(value);
            }

            protected override async Task<IList<TValue>> FetchResults(ICollection<TKey> requests)
            {
                //Sort requests and try to get what we can from the cache
                List<TValue> result = new List<TValue>();
                BlockRequest<TKey, TValue> freshRequests = endpoint.baseEndpoint.CreateRequestBuilder();
                bool hasFreshRequests = false;
                foreach (var r in requests)
                {
                    //Attempt to get it from cache
                    if (endpoint.TryGetFromCache(r, out TValue value))
                    {
                        //Got it from cache!
                        result.Add(value);
                    } else
                    {
                        //We'll need to query from the server...
                        freshRequests.ScheduleFetch(r);
                        hasFreshRequests = true;
                    }
                }

                //If we need to get anything from the base unit, request
                if (hasFreshRequests)
                {
                    //Query
                    List<TValue> newResults = new List<TValue>();
                    BlockResult<TKey, TValue> freshResults = await freshRequests.Submit();
                    freshResults.CopyTo(newResults);

                    //Add to cache
                    endpoint.InsertToCache(newResults);

                    //Add to main results
                    result.AddRange(newResults);
                }

                return result;
            }
        }
    }
}
