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
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> ConfirmStates([FromBody] ConfirmStateDto confirmStateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            await _userStateService.ConfirmStatesAsync(userId, confirmStateDto);
            return Ok();
        }
    }
}
