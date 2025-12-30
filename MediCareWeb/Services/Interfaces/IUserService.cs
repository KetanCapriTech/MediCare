using MediCare.Dto;
using MediCareApi.Models;

namespace MediCareWeb.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserById(long id);
    }
}
