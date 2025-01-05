using Elearning_Test.Data;
using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearning_Test.Controllers
{
    public class MyHomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICoursService _coursService;
        private readonly ICategorieService _categorieService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEtudiantService _etudiantService;

        public MyHomeController(
            ICoursService coursService,
            ICategorieService categorieService,
            UserManager<IdentityUser> userManager,
            IEtudiantService etudiantService,
            ApplicationDbContext context
        )
        {
            _coursService = coursService;
            _categorieService = categorieService;
            _userManager = userManager;
            _etudiantService = etudiantService;
            _context = context;
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

        [HttpGet]
        public async Task<IActionResult> InscriptionCours(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            var user = await _userManager.GetUserAsync(User);
            var etudiant = await _etudiantService.GetEtudiantByIdAsync(user!.Id);
            InscriptionPageViewModel viewModel = new()
            {
                Etudiant = etudiant!,
                Cours = cours!,
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> InscriptionCours(InscriptionPageViewModel IVM)
        {

           /* if (!ModelState.IsValid)
            {
                // Si le formulaire n'est pas valide, retournez à la vue avec les erreurs
                return View(IVM);
            }*/

            // Créer une instance de Enrollment
            var enrollment = new Enrollment
            {
                EtudiantId = IVM.Etudiant.Id,
                CoursId = IVM.Cours.Id,
                Progression = 0, // Progression initiale
                IsConnected = false, // Par défaut, l'étudiant n'est pas connecté
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Créer une instance de Payment
            var payment = new Payment
            {
                OwnerName = IVM.OwnerName,
                EtudiantId = IVM.Etudiant.Id,
                CoursId = IVM.Cours.Id,
                Amount = IVM.Cours.Price,
                PaymentDate = DateTime.UtcNow,
                CVC = IVM.Cvc,
                NumeroCarte = IVM.NumeroCarte,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Enregistrer les instances dans la base de données
            _context.Enrollments.Add(enrollment);
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Rediriger vers une autre page (par exemple, une page de confirmation)
            return RedirectToAction("homePage", "MyHome");
        }

    }
}
