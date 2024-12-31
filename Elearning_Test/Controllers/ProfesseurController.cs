using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompleteRoles.Controllers
{
    public class ProfesseurController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Professeur")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
