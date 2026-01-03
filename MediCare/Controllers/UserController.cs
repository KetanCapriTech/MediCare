using MediCare.CustomAttributes;
using MediCareApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MediCareApi.Controllers
{
    [MCAuthorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("get-user-by-id")]
        public async Task<IActionResult> GetUserById(long id)
        {
          var result =  await _userService.GetUserById(id);
              if (result == null)
              {
                 return BadRequest("user not found");
              }
          return Ok(result);
        }
    }
}
