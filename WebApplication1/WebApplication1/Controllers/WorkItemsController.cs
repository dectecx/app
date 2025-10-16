using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemRepository _workItemRepository;

        public WorkItemsController(IWorkItemRepository workItemRepository)
        {
            _workItemRepository = workItemRepository;
        }

        // GET: api/WorkItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkItem>>> GetWorkItems()
        {
            var workItems = await _workItemRepository.GetAllAsync();
            return Ok(workItems);
        }

        // GET: api/WorkItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItem>> GetWorkItem(int id)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);

            if (workItem == null)
            {
                return NotFound();
            }

            return Ok(workItem);
        }

        // POST: api/WorkItems
        [HttpPost]
        public async Task<ActionResult<WorkItem>> PostWorkItem(WorkItem workItem)
        {
            // Note: In a real app, you'd get the user from the HttpContext.
            workItem.CreatedUser = "System"; // Placeholder
            workItem.CreatedTime = DateTime.UtcNow;
            workItem.Status = "New"; // Default status
            
            await _workItemRepository.AddAsync(workItem);

            return CreatedAtAction(nameof(GetWorkItem), new { id = workItem.Id }, workItem);
        }

        // PUT: api/WorkItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkItem(int id, WorkItem workItem)
        {
            if (id != workItem.Id)
            {
                return BadRequest();
            }

            var existingWorkItem = await _workItemRepository.GetByIdAsync(id);
            if (existingWorkItem == null)
            {
                return NotFound();
            }

            // Note: In a real app, you'd get the user from the HttpContext.
            existingWorkItem.Title = workItem.Title;
            existingWorkItem.Description = workItem.Description;
            existingWorkItem.Status = workItem.Status;
            existingWorkItem.UpdatedUser = "System"; // Placeholder
            existingWorkItem.UpdatedTime = DateTime.UtcNow;

            await _workItemRepository.UpdateAsync(existingWorkItem);

            return NoContent();
        }

        // DELETE: api/WorkItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkItem(int id)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null)
            {
                return NotFound();
            }

            await _workItemRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}
