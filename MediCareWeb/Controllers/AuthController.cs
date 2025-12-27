using Microsoft.AspNetCore.Mvc;

namespace MediCareWeb.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
