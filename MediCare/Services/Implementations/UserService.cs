using MediCare.Dto.Auth;
using MediCare.Services.Implementations;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Repositories.Interfaces;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using MediCareUtilities;

namespace MediCareApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;

        public UserService(IUserRepository userRepository, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                var result = await _userRepository.GetUserByEmail(request.Email);

                if (result == null)
                {
                    return new AuthResponse
                    {
                        Message = "User not found",
                        IsSuccess = false,
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, result.Password))
                {
                    return new AuthResponse { Message = "Invalid password entered" };
                }

                return new AuthResponse
                {

                    Message = "Login success",
                    Token = _jwtHelper.GenrateJwtToken(result),
                    IsSuccess = true,
                    Email = result.Email,
                    UserId = result.Id
                };
            }
            catch (Exception ex) {

                return new AuthResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<long> RegisterUser(RegisterUserRequest request)
        {
            User user = new User() { 
            
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Address = request.Address,
                CreatedOn =DateTime.UtcNow,
                IsActive = true,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
            };

            var result = await _userRepository.CreateUserAsync(user);

            return result;
            
        }
    }
}
