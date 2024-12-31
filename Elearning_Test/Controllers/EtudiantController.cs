using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompleteRoles.Controllers
{
    public class EtudiantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Etudiant")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
