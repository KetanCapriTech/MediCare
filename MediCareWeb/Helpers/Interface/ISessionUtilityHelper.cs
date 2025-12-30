
using MediCareDto.Auth;

namespace MediCareWeb.Helpers.Interface
{
    public interface ISessionUtilityHelper
    {
        Task<bool> UpdateUserSessionAsync(long userId);
        Task<bool> SetUserSessionAsync(AuthResponse authResponse);
    }
}
