using MediCare.Dto;
using MediCare.Dto.Auth;
using MediCareApi.Models;
using MediCareDto.Auth;

namespace MediCareApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<long> CreateUserAsync(User user);
        Task<User> GetUserByEmailAsync(string request);
        Task<User> GetUserByIdAsync(long userId);
        Task<User> ApproveUserAsync(string email);
        Task<bool>UpdateUserAsync(User user);
    }
}
