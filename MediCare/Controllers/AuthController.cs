using MediCare.Dto;
using MediCare.Dto.Auth;
using MediCare.Models;
using MediCare.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtHelper _jwtHelper;

        public AuthController(IJwtHelper jwtHelper)
        {
            _jwtHelper = jwtHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest) {

            var SampleData = new User
            {
                Email = loginRequest.Email,
                PasswordHash  = loginRequest.Password
            };

            var token = _jwtHelper.GenrateJwtToken(SampleData);

            return Ok(new
            {
                Token = token,
            });
        }
    }
}
