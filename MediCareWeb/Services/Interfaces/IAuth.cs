using MediCare.Dto.Auth;
using MediCareDto.Auth;

namespace MediCareWeb.Services.Interfaces
{
    public interface IAuth
    {
        Task<AuthResponse> Login(LoginRequest model);
    }
}
