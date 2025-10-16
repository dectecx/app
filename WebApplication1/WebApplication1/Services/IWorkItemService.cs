using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IWorkItemService
    {
        Task<IEnumerable<WorkItemDto>> GetAllAsync();
        Task<WorkItemDto?> GetByIdAsync(int id);
        Task<WorkItemDto> CreateAsync(CreateWorkItemDto createDto, string createdByUser);
        Task UpdateAsync(int id, UpdateWorkItemDto updateDto, string updatedByUser);
        Task DeleteAsync(int id);
    }
}
