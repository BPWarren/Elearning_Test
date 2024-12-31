using Microsoft.AspNetCore.Mvc;

namespace Elearning_Test.Controllers
{
    public class MyHomeController : Controller
    {
        public IActionResult homePage()
        {
            return View();
        }
        public IActionResult contactUs()
        {
            return View();
        }
    }
}
