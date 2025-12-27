using MediCare.Dto.Auth;
using MediCareDto.Auth;
using MediCareWeb.Services.Interfaces;

namespace MediCareWeb.Services.Implementations
{
    public class Auth : IAuth
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _backendUrl;


        public Auth(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _backendUrl = _configuration["BackendUrl"] ?? throw new InvalidOperationException("BackendUrl not found in configuration.");

        }

        public async Task<AuthResponse> Login(LoginRequest model)
        {
            var result = await _httpClient.PostAsJsonAsync($"{_backendUrl}/api/auth/login", model);
            if (result != null) {

                return null;
            }
            return await result.Content.ReadFromJsonAsync<AuthResponse>();
        }
    }
}
