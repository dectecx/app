using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;

namespace WebApplication1.Controllers
{
    [Route("api/user/states")]
    [ApiController]
    public class UserStatesController : ControllerBase
    {
        // POST: api/user/states/confirm
        [HttpPost("confirm")]
        public IActionResult ConfirmStates([FromBody] ConfirmStateDto confirmStateDto)
        {
            // TODO: Implement logic to save the user's work item states.
            // This needs to be associated with the currently logged-in user.
            return Ok(new { message = "States confirmed successfully" });
        }
    }
}
