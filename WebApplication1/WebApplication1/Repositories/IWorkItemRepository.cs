using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem?> GetByIdAsync(int id);
        Task<IEnumerable<WorkItem>> GetAllAsync();
        Task AddAsync(WorkItem workItem);
        Task UpdateAsync(WorkItem workItem);
        Task DeleteAsync(int id);
    }
}
