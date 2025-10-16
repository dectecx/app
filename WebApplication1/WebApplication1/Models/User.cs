namespace WebApplication1.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public string? UpdatedUser { get; set; }
        public DateTime? UpdatedTime { get; set; }
        
        // 導航屬性
        public ICollection<UserWorkItemState>? UserWorkItemStates { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
