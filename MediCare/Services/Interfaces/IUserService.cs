using MediCare.Dto.Auth;
using MediCareDto.Auth;

namespace MediCareApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<long> RegisterUser(RegisterUserRequest request);
        Task<AuthResponse> Login(LoginRequest request);
    }
}
