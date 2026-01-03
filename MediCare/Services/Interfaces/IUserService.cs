using MediCare.Dto.Auth;
using MediCareApi.Models;
using MediCareDto.Auth;
using MediCareDto.Auth.Jwt;

namespace MediCareApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<long> RegisterUser(RegisterUserRequest request);
        Task<AuthResponse> Login(LoginRequest request);
        Task<User> GetUserById(long userId);
        Task<User> ApproveUser(string email);
        Task<bool> ValidateOtp(string email, string otp);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(RestPasswordDto model);
    }
}
