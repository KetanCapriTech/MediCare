using MediCare.Dto.Auth;
using MediCareApi.Models;
using MediCareDto.Auth;

namespace MediCareApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<long> RegisterUser(RegisterUserRequest request);
        Task<AuthResponse> Login(LoginRequest request);
        Task<User> GetUserById(long userId);
        Task<User> ApproveUser(string email);
    }
}
