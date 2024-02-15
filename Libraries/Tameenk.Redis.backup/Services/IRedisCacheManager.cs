using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Tameenk.Core.Caching;

namespace Tameenk.Redis
{
    public interface IRedisCacheManager : IDisposable //, ICacheManager
    {
        Task<T> GetAsync<T>(string key) where T : new();
        //Task SetAsync<T>(string key, T value, int duration = int.MaxValue);
        Task SetAsync<T>(string key, T value, int duration = int.MaxValue);
        Task<bool> TrySetAsync<T>(string key, T value, int duration = int.MaxValue, When when = When.Always);
        Task DeleteKeyAsync(string key);
        Task UpdateAsync<T>(string key, T value, int duration = int.MaxValue);
    }
}
