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
        //Vue pour pemettre à l'utilisateur de se connecter en tant que professeur ou en tant qu'étudiant
        public IActionResult ChoseConnection()
        {
            return View();
        }
    }
}
