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
            var workItems = await _workItemService.GetAllAsync();
            return Ok(workItems);
        }

        // GET: api/WorkItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItemDto>> GetWorkItem(int id)
        {
            var workItem = await _workItemService.GetByIdAsync(id);

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
            var user = GetCurrentUser();
            var newWorkItem = await _workItemService.CreateAsync(createDto, user);

            return CreatedAtAction(nameof(GetWorkItem), new { id = newWorkItem.Id }, newWorkItem);
        }

        // PUT: api/WorkItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkItem(int id, UpdateWorkItemDto updateDto)
        {
            var user = GetCurrentUser();
            await _workItemService.UpdateAsync(id, updateDto, user);
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
            // This is a simplified way to get the user's name from the claims.
            // In a real-world scenario, you might have a dedicated service for this.
            return User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous";
        }
    }
}
