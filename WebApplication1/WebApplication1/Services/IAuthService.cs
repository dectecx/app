using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto registerDto);
        Task<TokenResponseDto> LoginAsync(LoginDto loginDto);
        Task<TokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
        Task LogoutAsync(string? accessToken, string? refreshToken);
    }
}
