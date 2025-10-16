using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem?> GetByIdAsync(int id);
        Task<List<WorkItem>> GetByIdsAsync(IEnumerable<int> ids);
        Task<IEnumerable<WorkItem>> GetAllAsync();
        Task<WorkItem> AddAsync(WorkItem workItem);
        Task UpdateAsync(WorkItem workItem);
        Task DeleteAsync(int id);
    }
}
