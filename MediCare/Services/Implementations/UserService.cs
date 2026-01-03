using MediCare.Dto.Auth;
using MediCare.Services.Implementations;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Repositories.Interfaces;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using MediCareDto.Auth.Jwt;
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

        public async Task<User> ApproveUser(string email)
        {
            var userUpdated = await _userRepository.ApproveUserAsync(email);

            if (userUpdated == null) {

                return null;
            }

            return userUpdated;

        }

        public async Task<bool> ForgotPassword(string email)
        {
           var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null) { 
                return false;
            }
            var otpCode = new Random().Next(100000, 999999);
            user.OtpExpireTime = DateTime.UtcNow.AddMinutes(15);
            user.Otp = otpCode;
            
            var isUpdated = await _userRepository.UpdateUserAsync(user);

            if(isUpdated == false)
            {
                return false; 
            }

            _emailService.SendOtp(email, otpCode.ToString());

            return true;
        }

        public async Task<User> GetUserById(long userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if(user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResponse
                {
                    Message = "User not found",
                    IsSuccess = false
                };
            }

            try
            {
                bool isValid = BCrypt.Net.BCrypt.Verify(
                    request.Password,
                    user.Password
                );

                if (!isValid)
                {
                    return new AuthResponse
                    {
                        Message = "Invalid password entered",
                        IsSuccess = false
                    };
                }
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // Handles old/plain-text or corrupted passwords safely
                return new AuthResponse
                {
                    Message = "Password is invalid. Please reset your password.",
                    IsSuccess = false
                };
            }

            return new AuthResponse
            {
                Message = "Login success",
                Token = _jwtHelper.GenrateJwtToken(user),
                IsSuccess = true,
                Email = user.Email,
                UserId = user.Id
            };
        }

        public async Task<long> RegisterUser(RegisterUserRequest request)
        {
            // Determine if this user can be active immediately
            // In your Medicare app, NO ONE starts active. 
            // Patients need OTP. Others need Admin Approval.
            bool isPatient = (request.RoleId == (int)EnumRole.Patient);

            var otpCode = new Random().Next(100000, 999999);

            User user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Address = request.Address,
                CreatedOn = DateTime.UtcNow,
                IsActive = false, 
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RoleId = (short)request.RoleId,
                Otp = otpCode,
                OtpExpireTime = DateTime.UtcNow.AddMinutes(15)
            };

            var result = await _userRepository.CreateUserAsync(user);

            if(result == -1)
            {
                return -1;
            }

            // Handle Notifications based on Role
            if (isPatient)
            {
                // Flow A: Send OTP to Patient
                _emailService.SendOtp(user.Email, otpCode.ToString());
            }
            else
            {
                // Flow B: Admin, Manager, Doctor, Staff
                // All these roles need someone to "Approve" them
                string roleName = Enum.GetName(typeof(EnumRole), request.RoleId) ?? "User";
                _emailService.SendAdminApprovalRequest(
                    adminEmail: "ketanfundedev77@gmail.com",
                    userEmail: request.Email,
                    role: roleName
                );
            }

            return result;
        }

        public async Task<bool> ResetPassword(RestPasswordDto model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email!);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.ConfirmPassword);
            user.CreatedBy = user.Id;
            user.UpdatedOn = DateTime.UtcNow;

            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<bool> ValidateOtp(string email, string otp)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            // Check: Does OTP match AND is it still within the 15-minute window?
            if (user.Otp.ToString() == otp && user.OtpExpireTime > DateTime.UtcNow)
            {
                if (user.IsActive == false)
                {
                    user.IsActive = true;
                }

                // Save the change to the database
                var result = await _userRepository.UpdateUserAsync(user);
                if(result == false)
                {
                    return false;
                }
                return true;
            }

            return false;
        }

    }
}
