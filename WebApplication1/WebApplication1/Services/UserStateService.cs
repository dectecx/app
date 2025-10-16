using WebApplication1.DTOs;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{
    public class UserStateService : IUserStateService
    {
        private readonly IUserWorkItemStateRepository _repository;

        public UserStateService(IUserWorkItemStateRepository repository)
        {
            _repository = repository;
        }

        public async Task ConfirmStatesAsync(int userId, ConfirmStateDto confirmStateDto)
        {
            if (confirmStateDto.States == null || !confirmStateDto.States.Any())
            {
                // No states to update, so we can just return.
                return;
            }

            await _repository.UpsertStatesAsync(userId, confirmStateDto.States);
        }
    }
}
