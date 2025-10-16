using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<UserWorkItemState> UserWorkItemStates { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設定 UserWorkItemState 的主鍵
            modelBuilder.Entity<UserWorkItemState>()
                .HasKey(s => s.StateId);

            // 設定 UserWorkItemState 的外鍵關係
            modelBuilder.Entity<UserWorkItemState>()
                .HasOne(s => s.User)
                .WithMany(u => u.UserWorkItemStates)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkItemState>()
                .HasOne(s => s.WorkItem)
                .WithMany(w => w.UserWorkItemStates)
                .HasForeignKey(s => s.WorkItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // 設定 UserRole 的複合主鍵
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            // 設定 UserRole 的外鍵關係
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 建立唯一索引，防止一個使用者對同一個工作項目有多筆狀態
            modelBuilder.Entity<UserWorkItemState>()
                .HasIndex(s => new { s.UserId, s.WorkItemId })
                .IsUnique();

            // 設定 Role 的 Name 為唯一
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // 種子資料：建立預設角色
            modelBuilder.Entity<Role>().HasData(
                new Role 
                { 
                    RoleId = 1, 
                    Name = "User", 
                    Description = "前台使用者",
                    CreatedTime = DateTime.UtcNow
                },
                new Role 
                { 
                    RoleId = 2, 
                    Name = "Admin", 
                    Description = "後台管理員",
                    CreatedTime = DateTime.UtcNow
                }
            );
        }
    }
}
