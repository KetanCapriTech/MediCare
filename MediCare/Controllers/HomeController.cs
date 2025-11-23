using MediCare.CustomAttributes;
using Microsoft.AspNetCore.Mvc;

namespace MediCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        [MCAuthorize]
        [HttpGet("data")]
        public IActionResult Home()
        {
            return Ok("Test protected call!");
        }

        [HttpGet("public")]
        public IActionResult GetPublicData()
        {
            return Ok(new { Message = "This endpoint is public." });
        }
    }
}
