using MediCare.Dto.Auth;
using MediCareDto.Auth;
using MediCareWeb.Services.Interfaces;
using System.Net;
using System.Threading.Tasks;

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

        public async Task<AuthResponse?> Login(LoginRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_backendUrl}/api/v1/login", model);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }

        public async Task<AuthResponse> Register(RegisterUserRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"{_backendUrl}/api/v1/register",
                model
            );

            // Conflict - already exists
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                var error = await response.Content.ReadAsStringAsync();

                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = error
                };
            }

            // Accepted - pending approval
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var msg = await response.Content.ReadAsStringAsync();

                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = msg
                };
            }

            // Other failure
            if (!response.IsSuccessStatusCode)
            {
                return new AuthResponse
                {
                    IsSuccess = false,
                    Message = "Registration failed"
                };
            }

            // Success
            var data = await response.Content.ReadFromJsonAsync<AuthResponse>();

            return new AuthResponse
            {
                IsSuccess = true,
                Message = "Registration successful",
                UserId = data!.UserId
            };
        }

    }
}
