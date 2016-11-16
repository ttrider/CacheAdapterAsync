using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Xunit;
using Microsoft.Extensions.Caching.Distributed;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public async Task SimpleGetItemTest()
        {
            var mc = new MemoryCache(new MemoryCacheOptions());

            var targetKey = "test";
            var targetValue = 1;

            var index = targetValue;
            mc.Set(targetKey, targetValue);

            var result = await mc.GetOrAdd<int>(targetKey, (k) => Task.FromResult(index++));

            Assert.Equal(targetValue, result);
        }

        [Fact]
        public async Task SimpleAddItemTest()
        {
            var mc = new MemoryCache(new MemoryCacheOptions());

            var targetKey = "test";
            var targetValue = 1;
            var index = targetValue;

            var result = await mc.GetOrAdd<int>(targetKey, (k) => Task.FromResult(index++));

            Assert.Equal(targetValue, result);
            Assert.NotEqual(index, result);
        }

        [Fact]
        public async Task SimpleMultiRequestsTest()
        {
            var mc = new MemoryCache(new MemoryCacheOptions());

            var key = "test";
            var targetValue = 1;
            var index = targetValue;
            var tasks = Enumerable.Repeat(Task.Run(async () =>
            {
                return await mc.GetOrAdd(key, (k) => Task.FromResult(index++));
            }), 1000).ToList();


            await Task.WhenAll(tasks);
            foreach (var task in tasks)
            {
                Assert.Equal(targetValue, task.Result);
            }
            Assert.Equal(targetValue, mc.Get<int>(key));
        }
    }
}
