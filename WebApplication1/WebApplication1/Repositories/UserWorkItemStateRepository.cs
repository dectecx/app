using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public class UserWorkItemStateRepository : IUserWorkItemStateRepository
    {
        private readonly ApplicationDbContext _context;

        public UserWorkItemStateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpsertStatesAsync(int userId, IEnumerable<WorkItemStateDto> states)
        {
            var itemIds = states.Select(s => s.ItemId).ToList();
            var existingStates = await _context.UserWorkItemStates
                .Where(s => s.UserId == userId && itemIds.Contains(s.WorkItemId))
                .ToListAsync();

            foreach (var stateDto in states)
            {
                var existingState = existingStates.FirstOrDefault(s => s.WorkItemId == stateDto.ItemId);
                if (existingState != null)
                {
                    // Update existing state
                    existingState.IsChecked = stateDto.IsChecked;
                    existingState.IsConfirmed = true; // Confirmation implies this
                    existingState.UpdatedUser = userId.ToString(); // Or username, depending on policy
                    existingState.UpdatedTime = DateTime.UtcNow;
                }
                else
                {
                    // Create new state
                    var newState = new UserWorkItemState
                    {
                        UserId = userId,
                        WorkItemId = stateDto.ItemId,
                        IsChecked = stateDto.IsChecked,
                        IsConfirmed = true,
                        CreatedUser = userId.ToString(),
                        CreatedTime = DateTime.UtcNow
                    };
                    _context.UserWorkItemStates.Add(newState);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
