using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.DTOs;
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

        public async Task RegisterAsync(RegisterDto registerDto)
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
                CreatedUser = "System", // Placeholder
                CreatedTime = DateTime.UtcNow
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new InvalidCredentialsException("Invalid username or password.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            return _tokenService.GenerateTokens(claims);
        }

        public Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (string.IsNullOrEmpty(refreshTokenRequestDto.RefreshToken) || string.IsNullOrEmpty(refreshTokenRequestDto.AccessToken))
            {
                throw new InvalidRefreshTokenException("Access token and refresh token must be provided.");
            }

            // Check if the refresh token is blacklisted
            if (_cacheService.Exists(refreshTokenRequestDto.RefreshToken))
            {
                throw new InvalidRefreshTokenException("Invalid refresh token.");
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshTokenRequestDto.AccessToken);
            
            // Generate new tokens
            var newTokens = _tokenService.GenerateTokens(principal.Claims);

            // Blacklist the old refresh token
            var refreshTokenHandler = new JwtSecurityTokenHandler();
            var oldRefreshToken = refreshTokenHandler.ReadJwtToken(refreshTokenRequestDto.RefreshToken);
            var remainingLifetime = oldRefreshToken.ValidTo - DateTime.UtcNow;
            if (remainingLifetime > TimeSpan.Zero)
            {
                _cacheService.Set(refreshTokenRequestDto.RefreshToken, "blacklisted", remainingLifetime);
            }

            return Task.FromResult(newTokens);
        }

        public Task LogoutAsync(string? accessToken, string? refreshToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                var accessTokenHandler = new JwtSecurityTokenHandler();
                var accessTokenObj = accessTokenHandler.ReadJwtToken(accessToken);
                var remainingAccessLifetime = accessTokenObj.ValidTo - DateTime.UtcNow;
                if (remainingAccessLifetime > TimeSpan.Zero)
                {
                    _cacheService.Set(accessToken, "blacklisted", remainingAccessLifetime);
                }
            }

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var refreshTokenHandler = new JwtSecurityTokenHandler();
                var refreshTokenObj = refreshTokenHandler.ReadJwtToken(refreshToken);
                var remainingRefreshLifetime = refreshTokenObj.ValidTo - DateTime.UtcNow;
                if (remainingRefreshLifetime > TimeSpan.Zero)
                {
                    _cacheService.Set(refreshToken, "blacklisted", remainingRefreshLifetime);
                }
            }

            return Task.CompletedTask;
        }
    }
}
