using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly ApplicationDbContext _context;

        public WorkItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkItem?> GetByIdAsync(int id)
        {
            return await _context.WorkItems.FindAsync(id);
        }

        public async Task<IEnumerable<WorkItem>> GetAllAsync()
        {
            return await _context.WorkItems.ToListAsync();
        }

        public async Task AddAsync(WorkItem workItem)
        {
            await _context.WorkItems.AddAsync(workItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkItem workItem)
        {
            _context.WorkItems.Update(workItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var workItem = await _context.WorkItems.FindAsync(id);
            if (workItem != null)
            {
                _context.WorkItems.Remove(workItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
