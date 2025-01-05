using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elearning_Test.Controllers
{
    public class MyHomeController : Controller
    {
        private readonly ICoursService _coursService;
        private readonly ICategorieService _categorieService;
        public MyHomeController(
            ICoursService coursService
            , ICategorieService categorieService
        )
        {
            _coursService = coursService;
            _categorieService = categorieService;
        }
        [HttpGet]
        public async Task<IActionResult> homePage()
        {
            var ListCours = await _coursService.GetAllCoursAsync(3);
            var ListCategorie = await _categorieService.GetAllCategoriesAsync(4);
            HomePageViewModel viewModel = new()
            {
                Cours = ListCours,
                categories = ListCategorie
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

        [HttpGet]
        public async Task<IActionResult> VisisteCours(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            return View(cours);
        }
    }
}
