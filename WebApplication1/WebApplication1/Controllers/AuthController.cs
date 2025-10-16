using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var tokens = await _authService.LoginAsync(model);
            return Ok(tokens);
        }

        // POST: api/auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = await _authService.RegisterAsync(model);
            return Ok(new { message = "User registered successfully", userId = user.UserId });
        }

        // POST: api/auth/refresh
        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
        {
            var tokens = await _authService.RefreshTokenAsync(model);
            return Ok(tokens);
        }

        // POST: api/auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto model)
        {
            await _authService.LogoutAsync(model.AccessToken, model.RefreshToken);
            return Ok(new { message = "Logout successful" });
        }
    }
}
