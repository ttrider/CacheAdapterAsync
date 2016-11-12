using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace Microsoft.Extensions.Caching
{
    public static class Extensions
    {
        public static Task<T> GetOrAdd<T>(this IMemoryCache cache, Func<Task<T>> factoryMethod)
        {
            throw new NotImplementedException();
        }

        public static Task<T> GetOrAdd<T>(this IDistributedCache cache, Func<Task<T>> factoryMethod)
        {
            throw new NotImplementedException();
        }
    }
}
