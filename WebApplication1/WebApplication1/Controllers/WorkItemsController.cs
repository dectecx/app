using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _workItemService;

        public WorkItemsController(IWorkItemService workItemService)
        {
            _workItemService = workItemService;
        }

        // GET: api/WorkItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkItemDto>>> GetWorkItems()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var workItems = await _workItemService.GetAllAsync(userId);
            return Ok(workItems);
        }

        // GET: api/WorkItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItemDto>> GetWorkItem(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var workItem = await _workItemService.GetByIdAsync(id, userId);

            if (workItem == null)
            {
                return NotFound();
            }

            return Ok(workItem);
        }

        // POST: api/WorkItems
        [HttpPost]
        public async Task<ActionResult<WorkItemDto>> PostWorkItem(CreateWorkItemDto createDto)
        {
            var username = GetCurrentUser();
            var newWorkItem = await _workItemService.CreateAsync(createDto, username);

            return CreatedAtAction(nameof(GetWorkItem), new { id = newWorkItem.Id }, newWorkItem);
        }

        // PUT: api/WorkItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkItem(int id, UpdateWorkItemDto updateDto)
        {
            var username = GetCurrentUser();
            await _workItemService.UpdateAsync(id, updateDto, username);
            return NoContent();
        }

        // DELETE: api/WorkItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkItem(int id)
        {
            await _workItemService.DeleteAsync(id);
            return NoContent();
        }

        private string GetCurrentUser()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
