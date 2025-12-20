using MediCare.Dto.Auth;
using MediCare.Services.Implementations;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Repositories.Interfaces;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using MediCareUtilities;
using System.Security.Claims;

namespace MediCareApi.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IJwtHelper jwtHelper, IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
        }

        public Task<User> ApproveUser(string email)
        {
            var userUpdated = _userRepository.ApproveUserAsync(email);

            if (userUpdated == null) {

                return null;
            }

            return userUpdated;

        }

        public Task<User> GetUserById(long userId)
        {
            var user = _userRepository.GetUserByIDAsync(userId);
            if(user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                var result = await _userRepository.GetUserByEmailAsync(request.Email);

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
            catch (Exception ex)
            {

                return new AuthResponse
                {
                    Message = ex.Message,
                    IsSuccess = false
                };
            }
        }

        public async Task<long> RegisterUser(RegisterUserRequest request)
        {
            var forbidenRoles = new[] { (int)EnumRole.Admin, (int)EnumRole.Manager };
            string role = "";
            if (forbidenRoles.Contains(request.RoleId))
            {
                throw new UnauthorizedAccessException("This role requires administrative invitation.");
            }

            bool shouldBeActive = (request.RoleId == (int)EnumRole.Patient);

            User user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Address = request.Address,
                CreatedOn = DateTime.UtcNow,
                IsActive = shouldBeActive,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = request.RoleId,
            };

            if (request.RoleId == (int)EnumRole.Staff || request.RoleId == (int)EnumRole.Doctor)
            {
                role = request.RoleId == 3 ? EnumRole.Doctor.ToString() : EnumRole.Staff.ToString();
            }
            var result = await _userRepository.CreateUserAsync(user);

            // Notify Admin if a Doctor/Staff registered
            if (!shouldBeActive)
            {
                // _mailService.NotifyAdmin($"New {request.RoleId} registration pending approval: {request.Email}");
                _emailService.SendAdminApprovalRequest(
                    adminEmail: "ketanfundedev77@gmail.com",
                    userEmail: request.Email,
                    role: role
                );

                return -2;
            }

            return result;

        }
    }
}
