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
    }
}
