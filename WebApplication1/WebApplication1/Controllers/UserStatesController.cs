using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/user/states")]
    [ApiController]
    public class UserStatesController : ControllerBase
    {
        private readonly IUserStateService _userStateService;

        public UserStatesController(IUserStateService userStateService)
        {
            _userStateService = userStateService;
        }

        // POST: api/user/states/confirm
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmStates([FromBody] ConfirmStateDto confirmStateDto)
        {
            var userId = GetCurrentUserId();
            await _userStateService.ConfirmStatesAsync(userId, confirmStateDto);
            return NoContent();
        }

        private int GetCurrentUserId()
        {
            // This helper extracts the user ID from the token's claims.
            // It's important that this claim is present in the token.
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdValue, out var userId))
            {
                return userId;
            }
            // If the claim is missing or invalid, it indicates a problem with
            // the token or authentication setup. Throwing an exception is appropriate.
            throw new InvalidOperationException("User ID claim is missing or invalid.");
        }
    }
}
