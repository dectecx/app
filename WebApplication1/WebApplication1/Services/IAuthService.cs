using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
        Task LogoutAsync(string? accessToken, string? refreshToken);
    }
}
