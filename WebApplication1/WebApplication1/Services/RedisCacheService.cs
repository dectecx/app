using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        // 非同步方法
        public async Task SetAsync(string key, object value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _cache.SetAsync(key, bytes, options);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var bytes = await _cache.GetAsync(key);
            if (bytes == null)
                return default(T);

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var bytes = await _cache.GetAsync(key);
            return bytes != null;
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        // 同步方法保持向後相容性
        public void Set(string key, object value, TimeSpan expiration)
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            _cache.Set(key, bytes, options);
        }

        public T Get<T>(string key)
        {
            var bytes = _cache.Get(key);
            if (bytes == null)
                return default(T);

            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }

        public bool Exists(string key)
        {
            var bytes = _cache.Get(key);
            return bytes != null;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
