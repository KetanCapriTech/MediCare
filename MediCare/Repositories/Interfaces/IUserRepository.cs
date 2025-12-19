using MediCare.Dto.Auth;
using MediCareApi.Models;
using MediCareDto.Auth;

namespace MediCareApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<long> CreateUserAsync(User user);
        Task<User> GetUserByEmail(string request);
    }
}
