using MediCare.CustomAttributes;
using MediCare.Dto;
using MediCare.Dto.Auth;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using MediCareDto.Auth.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediCare.Controllers
{
    [ApiController]
    [Route("api/auth")]

    public class AuthController : Controller
    {
        private readonly IJwtHelper _jwtHelper;
        private readonly IUserService _userService;

        public AuthController(IJwtHelper jwtHelper, IUserService userService)
        {
            _jwtHelper = jwtHelper;
            _userService = userService;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request) {

            if (request == null)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUser(request);

            if (result == -1)
            {
                return Conflict(new
                {
                    success = false,
                    message = $"User with email {request.Email} already exists"
                });
            }
            else if (result == -2)
            {
                return Accepted(new
                {
                    success = false,
                    message = "Wait for admin approval"
                });
            }

            return Ok(new
            {
                success = true,
                message = "User registered successfully",
                Email = request.Email
            });

        }

        [MCAuthorize(1)]
        [HttpPatch("admin/approve-user/{Email}")]
        public async Task<IActionResult> ApproveUser(string email)
        {
            var user = await _userService.ApproveUser(email);
            if (user == null)
            {
                return NotFound();
            }

            return Ok($"User {user.Email} is now active!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userService.Login(loginRequest);

            if (user == null || !user.IsSuccess)
            {
                return BadRequest("Invalid credentials");
            }
            return Ok(user);
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpDto model)
        {
            if (model == null)
            {
                return BadRequest("Invalid OTP request");
            }
            var result = await _userService.ValidateOtp(model.email, model.otp);

            if (result == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email) {

            var result = await _userService.ForgotPassword(email);

            if (result == false)
            {
                return BadRequest("Email does not exist");
            }

            return Ok($"Otp sent on registered : {email}");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordDto([FromBody] RestPasswordDto model)
        {
            var result = await _userService.ResetPassword(model);

            if(result == false)
            {
               return BadRequest("Failed to Update Password");
            }
            return Ok(result);
        }
    }
}
