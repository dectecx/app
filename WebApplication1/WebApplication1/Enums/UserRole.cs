using System.Text.Json.Serialization;

namespace WebApplication1.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        User,      // 前台使用者
        Admin     // 後台管理員
    }
}