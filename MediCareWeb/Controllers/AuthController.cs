using MediCare.Dto.Auth;
using MediCareDto.Auth;
using MediCareWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MediCareWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuth _auth;
        public AuthController(IAuth auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid model" });
            }
            var result = await _auth.Login(model); 

            if (result == null)
            {
                return Json(new { success = false, message = "Invalid credentials" });
            }

            return Json(new
            {
                success = true,
                token = result.Token,
                message = "Login successful"
            });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid model" });
            }

            var result = await _auth.Register(model);

            if (result == null)
            {
                return StatusCode(500, new { message = "Failed to create user" });
            }

            if (result.Message.Contains("exist"))
            {
                return Conflict(new { message = "User already exists" }); // 409
            }

            if (result.Message.Contains("approve"))
            {
                return Accepted(new { message = "Wait for admin approval" }); // 202
            }

            return Ok(new { success = true, message = "Register successful" });
        }

    }
}
