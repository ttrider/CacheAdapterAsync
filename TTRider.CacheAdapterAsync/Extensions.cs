using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;

namespace Microsoft.Extensions.Caching
{
    public static class Extensions
    {
        static readonly ConcurrentDictionary<string, object> TaskDictionary = new ConcurrentDictionary<string, object>();

        public static async Task<T> GetOrAdd<T>(this IMemoryCache cache, string key, Func<string, Task<T>> factoryMethod)
        {
            object result;
            if (!cache.TryGetValue(key, out result))
            {
                var asyncLazy = (AsyncLazy<T>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<T>(k, async (kk) =>
                    {
                        var value = await factoryMethod(kk);
                        cache.Set(k, value); 
                        object oldAsync;
                        TaskDictionary.TryRemove(k, out oldAsync);
                        return value;
                    });

                    return newAsyncLazy;
                });

                return await asyncLazy.Value;
            }

            return (T)result;
        }

        public static async Task<byte[]> GetOrAdd(this IDistributedCache cache, string key, Func<string, Task<byte[]>> factoryMethod)
        {
            byte[] result = cache.Get(key);
            if(result == null)
            {
                var asyncLazy = (AsyncLazy<byte[]>)TaskDictionary.GetOrAdd(key, (k) =>
                {

                    var newAsyncLazy = new AsyncLazy<byte[]>(k, async (kk) =>
                    {
                        var value = await factoryMethod(kk);
                        cache.Set(k, value); 
                        object oldAsync;
                        TaskDictionary.TryRemove(k, out oldAsync);
                        return value;
                    });
                    return newAsyncLazy;
                });
                return await asyncLazy.Value;
            }
            return result;
        }
    }
}
