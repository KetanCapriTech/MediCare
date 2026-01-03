using MediCare.Dto.Auth;
using MediCareDto.Auth;
using MediCareDto.Auth.Jwt;

namespace MediCareWeb.Services.Interfaces
{
    public interface IAuth
    {
        Task<AuthResponse> Login(LoginRequest model);
        Task<AuthResponse?> Register(RegisterUserRequest model);
        Task<bool> ValidateOtp(OtpDto model);
        Task<bool> ForgotPassword(string email);
        Task<bool> ResetPassword(RestPasswordDto model);
    }
}
