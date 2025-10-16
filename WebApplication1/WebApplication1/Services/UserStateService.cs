using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserWorkItemStateRepository _userStateRepository;
        private readonly IWorkItemRepository _workItemRepository;
        private readonly ILogger<UserStateService> _logger;

        public UserStateService(
            IUserWorkItemStateRepository userStateRepository,
            IWorkItemRepository workItemRepository,
            ILogger<UserStateService> logger)
        {
            _userStateRepository = userStateRepository;
            _workItemRepository = workItemRepository;
            _logger = logger;
        }

        public async Task ConfirmStatesAsync(string userId, ConfirmStateDto confirmStateDto)
        {
            if (confirmStateDto.States == null || !confirmStateDto.States.Any())
            {
                _logger.LogWarning("ConfirmStatesAsync called with no states for user {UserId}.", userId);
                return;
            }

            var incomingWorkItemIds = confirmStateDto.States.Select(s => s.WorkItemId).Distinct().ToList();
            var existingWorkItems = await _workItemRepository.GetByIdsAsync(incomingWorkItemIds);
            var existingWorkItemIds = new HashSet<int>(existingWorkItems.Select(w => w.Id));

            var invalidIds = incomingWorkItemIds.Where(id => !existingWorkItemIds.Contains(id)).ToList();
            if (invalidIds.Any())
            {
                var invalidIdsString = string.Join(", ", invalidIds);
                _logger.LogWarning("ConfirmStatesAsync failed due to invalid WorkItem IDs for user {UserId}. Invalid IDs: {InvalidIds}", userId, invalidIdsString);
                // Consider creating a custom, more specific exception
                throw new BadHttpRequestException($"The following WorkItem IDs do not exist: {invalidIdsString}");
            }

            var states = confirmStateDto.States.Select(s => new UserWorkItemState
            {
                UserId = int.Parse(userId),
                WorkItemId = s.WorkItemId,
                IsConfirmed = s.IsConfirmed,
                // IsChecked will be implicitly true if a user confirms it.
                // Or you can have separate logic for checking and confirming.
                IsChecked = true 
            }).ToList();

            await _userStateRepository.UpsertStatesAsync(int.Parse(userId), states);
        }
    }
}
