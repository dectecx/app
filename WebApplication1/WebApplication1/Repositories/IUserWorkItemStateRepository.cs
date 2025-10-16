using WebApplication1.DTOs;

namespace WebApplication1.Repositories
{
    public interface IUserWorkItemStateRepository
    {
        Task UpsertStatesAsync(int userId, IEnumerable<WorkItemStateDto> states);
    }
}
