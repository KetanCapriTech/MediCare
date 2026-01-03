using MediCare.Dto;
using MediCareApi.Models;
using MediCareDto.Auth;

namespace MediCareWeb.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserById(long id);
    }
}
