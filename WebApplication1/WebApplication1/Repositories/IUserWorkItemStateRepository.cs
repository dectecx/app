using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IUserWorkItemStateRepository
    {
        Task UpsertStatesAsync(int userId, IEnumerable<UserWorkItemState> states);
    }
}
