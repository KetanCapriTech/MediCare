using MediCare.Dto;
using MediCareDto.Auth;
using MediCareWeb.Services.Interfaces;
using System.Threading.Tasks;

namespace MediCareWeb.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _backendUrl;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _backendUrl = _configuration["BackendUrl"] ?? throw new InvalidOperationException("Backend url not found in configurations");
        }

        public async Task<UserDto?> GetUserById(long id)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_backendUrl}/api/v1/user/get-user-by-id", id);

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }

    }
}
