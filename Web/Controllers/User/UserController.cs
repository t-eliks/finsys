using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.User
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View("UserMainPage");
        }
    }
}