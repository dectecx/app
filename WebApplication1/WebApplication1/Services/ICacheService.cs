namespace WebApplication1.Services
{
    public interface ICacheService
    {
        void Set(string key, object value, TimeSpan expiration);
        T Get<T>(string key);
        bool Exists(string key);
    }
}
