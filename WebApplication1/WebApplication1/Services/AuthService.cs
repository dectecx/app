using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Enums;
using WebApplication1.Exceptions;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public AuthService(
            ApplicationDbContext context,
            ITokenService tokenService,
            IConfiguration configuration,
            ICacheService cacheService)
        {
            _context = context;
            _tokenService = tokenService;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Username == registerDto.Username);
            if (userExists)
            {
                throw new UserAlreadyExistsException("User with this username already exists.");
            }

            var user = new User
            {
                Username = registerDto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                CreatedUser = registerDto.Username,
                CreatedTime = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // 為新使用者分配預設的 User 角色
            var userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole != null)
            {
                var userRoleAssignment = new Models.UserRole
                {
                    UserId = user.UserId,
                    RoleId = userRole.RoleId,
                    AssignedTime = DateTime.UtcNow,
                    AssignedBy = registerDto.Username
                };
                _context.UserRoles.Add(userRoleAssignment);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException("Invalid username or password.");
            }

            // 取得使用者的所有角色
            var userRoles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var roleClaims = userRoles.Select(role => new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? "Unknown")
            }.Concat(roleClaims).ToArray();

            return _tokenService.GenerateTokens(claims);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (string.IsNullOrEmpty(refreshTokenRequestDto.RefreshToken) || string.IsNullOrEmpty(refreshTokenRequestDto.AccessToken))
            {
                throw new InvalidRefreshTokenException("Access token and refresh token must be provided.");
            }

            // Check if the refresh token is blacklisted
            if (await _cacheService.ExistsAsync(refreshTokenRequestDto.RefreshToken))
            {
                throw new InvalidRefreshTokenException("Invalid refresh token (blacklisted).");
            }
            
            // Validate the refresh token itself
            var refreshTokenHandler = new JwtSecurityTokenHandler();
            var refreshTokenPrincipal = refreshTokenHandler.ReadJwtToken(refreshTokenRequestDto.RefreshToken);
            if (refreshTokenPrincipal.ValidTo < DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException("Invalid refresh token (expired).");
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenRequestDto.AccessToken);
            
            // Generate new tokens
            var newTokens = _tokenService.GenerateTokens(principal.Claims);

            // Blacklist the old refresh token
            var remainingLifetime = refreshTokenPrincipal.ValidTo - DateTime.UtcNow;
            if (remainingLifetime > TimeSpan.Zero)
            {
                await _cacheService.SetAsync(refreshTokenRequestDto.RefreshToken, "blacklisted", remainingLifetime);
            }

            return newTokens;
        }

        public async Task LogoutAsync(string? accessToken, string? refreshToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessTokenHandler = new JwtSecurityTokenHandler();
                var accessTokenObj = accessTokenHandler.ReadJwtToken(accessToken);
                var remainingAccessLifetime = accessTokenObj.ValidTo - DateTime.UtcNow;
                if (remainingAccessLifetime > TimeSpan.Zero)
                {
                    await _cacheService.SetAsync(accessToken, "blacklisted", remainingAccessLifetime);
                }
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var refreshTokenHandler = new JwtSecurityTokenHandler();
                var refreshTokenObj = refreshTokenHandler.ReadJwtToken(refreshToken);
                var remainingRefreshLifetime = refreshTokenObj.ValidTo - DateTime.UtcNow;
                if (remainingRefreshLifetime > TimeSpan.Zero)
                {
                    await _cacheService.SetAsync(refreshToken, "blacklisted", remainingRefreshLifetime);
                }
            }
        }
    }
}
