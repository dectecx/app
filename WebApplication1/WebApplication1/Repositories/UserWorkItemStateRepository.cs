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

        public async Task UpsertStatesAsync(int userId, IEnumerable<UserWorkItemState> statesToUpdate)
        {
            var workItemIds = statesToUpdate.Select(s => s.WorkItemId).ToList();
            var existingStates = await _context.UserWorkItemStates
                .Where(s => s.UserId == userId && workItemIds.Contains(s.WorkItemId))
                .ToListAsync();

            foreach (var stateToUpdate in statesToUpdate)
            {
                var existingState = existingStates.FirstOrDefault(s => s.WorkItemId == stateToUpdate.WorkItemId);
                if (existingState != null)
                {
                    // Update existing state
                    existingState.IsConfirmed = stateToUpdate.IsConfirmed;
                    existingState.IsChecked = true; // Or handle as needed
                    existingState.UpdatedUser = userId.ToString(); // Assuming UpdatedUser is the user's ID
                    existingState.UpdatedTime = DateTime.UtcNow;
                }
                else
                {
                    // Add new state
                    stateToUpdate.UserId = userId;
                    stateToUpdate.CreatedUser = userId.ToString(); // Assuming CreatedUser is the user's ID
                    stateToUpdate.CreatedTime = DateTime.UtcNow;
                    _context.UserWorkItemStates.Add(stateToUpdate);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
