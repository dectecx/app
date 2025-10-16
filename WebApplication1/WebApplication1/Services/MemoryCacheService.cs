using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        // 非同步方法
        public Task SetAsync(string key, object value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return Task.FromResult(value);
        }

        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out _));
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        // 同步方法保持向後相容性
        public void Set(string key, object value, TimeSpan expiration)
        {
            _cache.Set(key, value, expiration);
        }

        public T Get<T>(string key)
        {
            _cache.TryGetValue(key, out T value);
            return value;
        }

        public bool Exists(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
