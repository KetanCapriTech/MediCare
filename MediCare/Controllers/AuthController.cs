using MediCare.CustomAttributes;
using MediCare.Dto;
using MediCare.Dto.Auth;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            
            if(request == null)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUser(request);

            if(result == -1)
            {
                return BadRequest($"User with email : {request.Email} is already exist");
            }
            else if(result == -2)
            {
                return Ok($"Wait for admin to approve the request");
            }

            return Ok(result);

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
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {

            var user = _userService.Login(loginRequest);

            return Ok(user.Result);
        }
    }
}
