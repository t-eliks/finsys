using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View("UserMainPage");
        }
    }
}