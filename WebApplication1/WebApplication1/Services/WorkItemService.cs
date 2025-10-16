using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _workItemRepository;

        public WorkItemService(IWorkItemRepository workItemRepository)
        {
            _workItemRepository = workItemRepository;
        }

        public async Task<IEnumerable<WorkItemDto>> GetAllAsync()
        {
            var workItems = await _workItemRepository.GetAllAsync();
            return workItems.Select(MapToDto);
        }

        public async Task<WorkItemDto?> GetByIdAsync(int id)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            return workItem == null ? null : MapToDto(workItem);
        }

        public async Task<WorkItemDto> CreateAsync(CreateWorkItemDto createDto, string createdByUser)
        {
            var workItem = new WorkItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                Status = "New", // Default status
                CreatedUser = createdByUser,
                CreatedTime = DateTime.UtcNow
            };

            await _workItemRepository.AddAsync(workItem);
            return MapToDto(workItem);
        }

        public async Task UpdateAsync(int id, UpdateWorkItemDto updateDto, string updatedByUser)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null)
            {
                // In a real app, you might throw a NotFoundException here
                // which would be handled by the exception handling middleware.
                return;
            }

            workItem.Title = updateDto.Title ?? workItem.Title;
            workItem.Description = updateDto.Description ?? workItem.Description;
            workItem.Status = updateDto.Status ?? workItem.Status;
            workItem.UpdatedUser = updatedByUser;
            workItem.UpdatedTime = DateTime.UtcNow;

            await _workItemRepository.UpdateAsync(workItem);
        }

        public async Task DeleteAsync(int id)
        {
            await _workItemRepository.DeleteAsync(id);
        }

        private static WorkItemDto MapToDto(WorkItem workItem)
        {
            return new WorkItemDto
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                Status = workItem.Status,
                CreatedUser = workItem.CreatedUser,
                CreatedTime = workItem.CreatedTime,
                UpdatedUser = workItem.UpdatedUser,
                UpdatedTime = workItem.UpdatedTime
            };
        }
    }
}
