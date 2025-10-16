using WebApplication1.DTOs;
using WebApplication1.Enums;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IUserWorkItemStateRepository _userStateRepository;

        public WorkItemService(IWorkItemRepository workItemRepository, IUserWorkItemStateRepository userStateRepository)
        {
            _workItemRepository = workItemRepository;
            _userStateRepository = userStateRepository;
        }

        public async Task<IEnumerable<WorkItemDto>> GetAllAsync(string userId)
        {
            var workItems = await _workItemRepository.GetAllAsync();
            var userStates = await _userStateRepository.GetStatesByUserIdAsync(int.Parse(userId));
            var userStatesLookup = userStates.ToDictionary(s => s.WorkItemId);

            return workItems.Select(wi => MapToDto(wi, userStatesLookup.GetValueOrDefault(wi.Id)));
        }

        public async Task<WorkItemDto?> GetByIdAsync(int id, string userId)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null) return null;

            var userState = await _userStateRepository.GetStateAsync(int.Parse(userId), id);
            return MapToDto(workItem, userState);
        }

        public async Task<WorkItemDto> CreateAsync(CreateWorkItemDto createDto, string createdBy)
        {
            var workItem = new WorkItem
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedUser = createdBy,
                CreatedTime = DateTime.UtcNow
            };

            var newWorkItem = await _workItemRepository.AddAsync(workItem);
            
            // By default, a new item has a pending status for the creator, but no explicit state is created yet.
            // The status will be derived as Pending because no UserWorkItemState exists.
            return MapToDto(newWorkItem, null);
        }

        public async Task UpdateAsync(int id, UpdateWorkItemDto updateDto, string updatedBy)
        {
            var workItem = await _workItemRepository.GetByIdAsync(id);
            if (workItem == null)
            {
                return;
            }

            workItem.Title = updateDto.Title ?? workItem.Title;
            workItem.Description = updateDto.Description ?? workItem.Description;
            workItem.UpdatedUser = updatedBy;
            workItem.UpdatedTime = DateTime.UtcNow;

            await _workItemRepository.UpdateAsync(workItem);
        }

        public async Task DeleteAsync(int id)
        {
            await _workItemRepository.DeleteAsync(id);
        }

        private WorkItemDto MapToDto(WorkItem workItem, UserWorkItemState? userState)
        {
            return new WorkItemDto
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                UserStatus = userState != null && userState.IsConfirmed ? ConfirmationStatus.Confirmed : ConfirmationStatus.Pending,
                CreatedUser = workItem.CreatedUser,
                CreatedTime = workItem.CreatedTime,
                UpdatedUser = workItem.UpdatedUser,
                UpdatedTime = workItem.UpdatedTime
            };
        }
    }
}
