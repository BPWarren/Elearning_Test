using Microsoft.AspNetCore.Mvc;

namespace Elearning_Test.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult sidebar_style2()
        {
            return View();
        }
        public IActionResult buttons()
        {
            return View("components/buttons");
        }

    }
}
