using MediCare.Dto.Auth;
using MediCare.Helpers.Interface;
using MediCareDto.Auth;
using MediCareDto.Auth.Jwt;
using MediCareWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediCareWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuth _auth;
        private readonly ISessionHelper _sessionHelper;

        public AuthController(IAuth auth,ISessionHelper session)
        {
            _auth = auth;
            _sessionHelper = session;
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
            _sessionHelper.SetObject("UserRegistration", result);

            return Ok(new { success = true, message = "Register successful" });
        }

        [HttpGet]
        public IActionResult ValidateOtp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOtp(OtpDto model)
        {
            var userInfo = _sessionHelper.GetObject<AuthResponse>("UserRegistration");

            var forgotPasswordEmail = _sessionHelper.GetString("forgotPasswordEmail");

            OtpDto otpDto = new OtpDto();

            if(forgotPasswordEmail != null)
            {
                otpDto.email = forgotPasswordEmail;
            }

            otpDto = new()
            {
                email = string.IsNullOrWhiteSpace(userInfo?.Email) ? otpDto.email : userInfo.Email,
                otp = model.otp
            };
            bool result = await _auth.ValidateOtp(otpDto);

            if (result == false)
            {
                return Json(new
                {
                    success = false,
                    message = "invalid otp"
                });
            }

            return Ok(new { success = true, message = "Otp verified successfully" });
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _auth.ForgotPassword(email);
            if (result == false) {
                return Json(new
                {
                    success = false,
                    message = "Email does not exist"
                });
            }
            _sessionHelper.SetString("forgotPasswordEmail", email);
            return Ok(new { success = true, message = $"otp sent on {email}" });
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(RestPasswordDto model)
        {
           var email = _sessionHelper.GetString("forgotPasswordEmail");

            if (email == null) {

                return Json(new
                {
                    success = false,
                    message = "Email is required"
                });
            }
            model.Email = email;

            bool isRest = await _auth.ResetPassword(model);

            if (!isRest) {
                return Json(new
                {
                    success = false,
                    message = "Failed to reset Password"
                });
            }

            return Ok(new { success = true , message = "Password reset successfully"});
        }
    }
}
