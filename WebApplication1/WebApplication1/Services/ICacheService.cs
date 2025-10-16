namespace WebApplication1.Services
{
    public interface ICacheService
    {
        Task SetAsync(string key, object value, TimeSpan expiration);
        Task<T> GetAsync<T>(string key);
        Task<bool> ExistsAsync(string key);
        Task RemoveAsync(string key);
        
        // 同步方法保持向後相容性
        void Set(string key, object value, TimeSpan expiration);
        T Get<T>(string key);
        bool Exists(string key);
        void Remove(string key);
    }
}
