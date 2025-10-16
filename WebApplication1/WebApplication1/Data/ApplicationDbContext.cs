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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly configure the primary key for UserWorkItemState
            modelBuilder.Entity<UserWorkItemState>()
                .HasKey(s => s.StateId);

            // According to README, UserWorkItemStates is a junction table with its own primary key.
            // Here we define the relationships between the tables.

            modelBuilder.Entity<UserWorkItemState>()
                .HasOne(s => s.User)
                .WithMany(u => u.UserWorkItemStates)
                .HasForeignKey(s => s.UserId);

            modelBuilder.Entity<UserWorkItemState>()
                .HasOne(s => s.WorkItem)
                .WithMany(w => w.UserWorkItemStates)
                .HasForeignKey(s => s.WorkItemId);
        }
    }
}
