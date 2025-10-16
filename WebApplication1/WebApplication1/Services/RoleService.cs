using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IRoleService
    {
        Task<bool> AssignRoleToUserAsync(int userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(int userId, string roleName);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<bool> IsUserInRoleAsync(int userId, string roleName);
    }

    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RoleService> _logger;

        public RoleService(ApplicationDbContext context, ILogger<RoleService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, string roleName)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                {
                    _logger.LogWarning("Role {RoleName} not found", roleName);
                    return false;
                }

                var existingAssignment = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

                if (existingAssignment != null)
                {
                    _logger.LogInformation("User {UserId} already has role {RoleName}", userId, roleName);
                    return true; // 已經有這個角色了
                }

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = role.RoleId,
                    AssignedTime = DateTime.UtcNow,
                    AssignedBy = "System" // 可以從當前使用者取得
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully assigned role {RoleName} to user {UserId}", roleName, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", roleName, userId);
                return false;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, string roleName)
        {
            try
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
                if (role == null)
                {
                    _logger.LogWarning("Role {RoleName} not found", roleName);
                    return false;
                }

                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == role.RoleId);

                if (userRole == null)
                {
                    _logger.LogInformation("User {UserId} does not have role {RoleName}", userId, roleName);
                    return true; // 本來就沒有這個角色
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully removed role {RoleName} from user {UserId}", roleName, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", roleName, userId);
                return false;
            }
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.Role.Name)
                    .ToListAsync();

                return userRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
                return new List<string>();
            }
        }

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            try
            {
                var hasRole = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);

                return hasRole;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user {UserId} has role {RoleName}", userId, roleName);
                return false;
            }
        }
    }
}
