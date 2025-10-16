using System.Security.Claims;
using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface ITokenService
    {
        TokenResponseDto GenerateTokens(IEnumerable<Claim> claims);
    }
}
