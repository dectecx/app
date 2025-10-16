using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IUserStateService
    {
        Task ConfirmStatesAsync(int userId, ConfirmStateDto confirmStateDto);
    }
}
