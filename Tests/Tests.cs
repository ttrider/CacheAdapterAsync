using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public async Task SimpleTest()
        {
            var mc = new MemoryCache(new MemoryCacheOptions());

            // create multiple parallel requests.
            // if the code works properly, all of them 
            // should get the same value and the 
            // memory cache instance should have
            // the only one entry with the same value

            var key = "any";
            var targetValue = 1972;
            var index = targetValue;
            var tasks = Enumerable.Repeat(Task.Run(async () =>
            {
                return await mc.GetOrAdd(key, (k) => Task.FromResult(index++));
            }), 100).ToList();


            // wait until all tasks get their value
            await Task.WhenAll(tasks);

            foreach (var task in tasks)
            {
                Assert.Equal(targetValue, task.Result);
            }

            Assert.Equal(targetValue, mc.Get<int>(key));



        }
    }
}
