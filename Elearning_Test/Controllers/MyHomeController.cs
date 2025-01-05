using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elearning_Test.Controllers
{
    public class MyHomeController : Controller
    {
        private readonly ICoursService _coursService;
        public MyHomeController(
            ICoursService coursService
        )
        {
            _coursService = coursService;
        }
        [HttpGet]
        public async Task<IActionResult> homePage()
        {
            var ListCours = await _coursService.GetAllCoursAsync(6);
            HomePageViewModel viewModel = new()
            {
                Cours = ListCours,
            };
            return View(viewModel);
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
        [HttpGet]
        public async Task<IActionResult> CoursesPage()
        {
            var ListCours = await _coursService.GetAllCoursAsync();
            HomePageViewModel viewModel = new()
            {
                Cours = ListCours,
            };
            return View(viewModel);
        }
    }
}
