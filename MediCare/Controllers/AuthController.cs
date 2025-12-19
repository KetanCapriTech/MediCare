using MediCare.Dto;
using MediCare.Dto.Auth;
using MediCare.Services.Interfaces;
using MediCareApi.Models;
using MediCareApi.Services.Interfaces;
using MediCareDto.Auth;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Controllers
{
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
        public async Task<IActionResult> Register(RegisterUserRequest request) { 
            
            if(request == null)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterUser(request);

            return Ok(result);

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {

            var user = _userService.Login(loginRequest);

            return Ok(user.Result);
        }
    }
}
