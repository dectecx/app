namespace WebApplication1.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        
        // 導航屬性
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedTime { get; set; }
        public string? AssignedBy { get; set; }
        
        // 導航屬性
        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }
}
