using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
#if DEBUG
    [ApiController]
    [Route("api/[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;
        private readonly ILogger<TestDataController> _logger;
        private readonly IWebHostEnvironment _environment;

        public TestDataController(
            ApplicationDbContext context,
            IAuthService authService,
            ILogger<TestDataController> logger,
            IWebHostEnvironment environment)
        {
            _context = context;
            _authService = authService;
            _logger = logger;
            _environment = environment;
        }

        private bool IsDevelopmentOrDebug()
        {
            return _environment.IsDevelopment() || _environment.IsEnvironment("Debug");
        }

        [AllowAnonymous]
        [HttpPost("create-test-users")]
        public async Task<IActionResult> CreateTestUsers()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                _logger.LogInformation("開始建立測試使用者...");

                // 檢查是否已存在測試使用者
                var existingAdmin = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "user");

                bool adminCreated = false;
                bool userCreated = false;
                var messages = new List<string>();

                // 建立 Admin 使用者
                if (existingAdmin == null)
                {
                    var adminUser = new User
                    {
                        Username = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                        CreatedUser = "system",
                        CreatedTime = DateTime.UtcNow
                    };

                    await _context.Users.AddAsync(adminUser);
                    await _context.SaveChangesAsync();

                    // 為 Admin 分配 Admin 角色
                    var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                    if (adminRole != null)
                    {
                        var adminRoleAssignment = new Models.UserRole
                        {
                            UserId = adminUser.UserId,
                            RoleId = adminRole.RoleId,
                            AssignedTime = DateTime.UtcNow,
                            AssignedBy = "system"
                        };
                        _context.UserRoles.Add(adminRoleAssignment);
                    }

                    adminCreated = true;
                    messages.Add("Admin 使用者已建立");
                    _logger.LogInformation("Admin 使用者建立成功");
                }
                else
                {
                    messages.Add("Admin 使用者已存在");
                    _logger.LogInformation("Admin 使用者已存在");
                }

                // 建立 User 使用者
                if (existingUser == null)
                {
                    var normalUser = new User
                    {
                        Username = "user",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                        CreatedUser = "system",
                        CreatedTime = DateTime.UtcNow
                    };

                    await _context.Users.AddAsync(normalUser);
                    await _context.SaveChangesAsync();

                    // 為 User 分配 User 角色
                    var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                    if (userRole != null)
                    {
                        var userRoleAssignment = new Models.UserRole
                        {
                            UserId = normalUser.UserId,
                            RoleId = userRole.RoleId,
                            AssignedTime = DateTime.UtcNow,
                            AssignedBy = "system"
                        };
                        _context.UserRoles.Add(userRoleAssignment);
                    }

                    userCreated = true;
                    messages.Add("User 使用者已建立");
                    _logger.LogInformation("User 使用者建立成功");
                }
                else
                {
                    messages.Add("User 使用者已存在");
                    _logger.LogInformation("User 使用者已存在");
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        AdminCreated = adminCreated,
                        UserCreated = userCreated,
                        AdminExists = existingAdmin != null,
                        UserExists = existingUser != null,
                        Messages = messages
                    },
                    TestCredentials = new
                    {
                        Admin = new { Username = "admin", Password = "123456" },
                        User = new { Username = "user", Password = "123456" }
                    },
                    Message = string.Join(", ", messages)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "建立測試使用者時發生錯誤");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "建立測試使用者失敗",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("login-test-admin")]
        public async Task<IActionResult> LoginTestAdmin()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var loginDto = new LoginDto
                {
                    Username = "admin",
                    Password = "123456"
                };

                var tokenResponse = await _authService.LoginAsync(loginDto);
                
                return Ok(new
                {
                    Success = true,
                    Message = "Admin 登入成功",
                    Token = tokenResponse,
                    UserInfo = new
                    {
                        Username = "admin",
                        Role = "Admin"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin 登入失敗");
                return BadRequest(new
                {
                    Success = false,
                    Message = "Admin 登入失敗",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("login-test-user")]
        public async Task<IActionResult> LoginTestUser()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var loginDto = new LoginDto
                {
                    Username = "user",
                    Password = "123456"
                };

                var tokenResponse = await _authService.LoginAsync(loginDto);
                
                return Ok(new
                {
                    Success = true,
                    Message = "User 登入成功",
                    Token = tokenResponse,
                    UserInfo = new
                    {
                        Username = "user",
                        Role = "User"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "User 登入失敗");
                return BadRequest(new
                {
                    Success = false,
                    Message = "User 登入失敗",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("test-users-status")]
        public async Task<IActionResult> GetTestUsersStatus()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var adminUser = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == "admin");

                var normalUser = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == "user");

                // 建立 Admin 使用者資訊
                object adminInfo;
                if (adminUser != null)
                {
                    adminInfo = new
                    {
                        Exists = true,
                        Username = adminUser.Username,
                        Roles = adminUser.UserRoles.Select(ur => ur.Role.Name).ToList(),
                        CreatedTime = adminUser.CreatedTime
                    };
                }
                else
                {
                    adminInfo = new { Exists = false };
                }

                // 建立 User 使用者資訊
                object userInfo;
                if (normalUser != null)
                {
                    userInfo = new
                    {
                        Exists = true,
                        Username = normalUser.Username,
                        Roles = normalUser.UserRoles.Select(ur => ur.Role.Name).ToList(),
                        CreatedTime = normalUser.CreatedTime
                    };
                }
                else
                {
                    userInfo = new { Exists = false };
                }

                return Ok(new
                {
                    Success = true,
                    Data = new
                    {
                        Admin = adminInfo,
                        User = userInfo
                    },
                    TestCredentials = new
                    {
                        Admin = new { Username = "admin", Password = "123456" },
                        User = new { Username = "user", Password = "123456" }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得測試使用者狀態時發生錯誤");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "取得測試使用者狀態失敗",
                    Error = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete-test-users")]
        public async Task<IActionResult> DeleteTestUsers()
        {
            if (!IsDevelopmentOrDebug())
            {
                return NotFound();
            }

            try
            {
                var adminUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
                var normalUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == "user");

                var deletedCount = 0;

                if (adminUser != null)
                {
                    _context.Users.Remove(adminUser);
                    deletedCount++;
                }

                if (normalUser != null)
                {
                    _context.Users.Remove(normalUser);
                    deletedCount++;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    Message = $"已刪除 {deletedCount} 個測試使用者",
                    DeletedCount = deletedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刪除測試使用者時發生錯誤");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "刪除測試使用者失敗",
                    Error = ex.Message
                });
            }
        }
    }
#endif
}
