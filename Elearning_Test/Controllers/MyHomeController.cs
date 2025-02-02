﻿using Elearning_Test.Data;
using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IEnrollmentService _enrollmentService;

        public MyHomeController(
            ICoursService coursService,
            ICategorieService categorieService,
            UserManager<IdentityUser> userManager,
            IEtudiantService etudiantService,
            IEnrollmentService enrollmentService,
            ApplicationDbContext context
        )
        {
            _coursService = coursService;
            _categorieService = categorieService;
            _userManager = userManager;
            _etudiantService = etudiantService;
            _context = context;
            _enrollmentService = enrollmentService;
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

        //[HttpGet]
        //public async Task<IActionResult> InscriptionCours(int id)
        //{
        //    var cours = await _coursService.GetCoursByIdAsync(id);
        //    var user = await _userManager.GetUserAsync(User);
        //    var etudiant = await _etudiantService.GetEtudiantByIdAsync(user!.Id);
        //    InscriptionPageViewModel viewModel = new()
        //    {
        //        Etudiant = etudiant!,
        //        Cours = cours!,
        //    };
        //    return View(viewModel);
        //}

        //[HttpPost]
        //public async Task<IActionResult> InscriptionCours(InscriptionPageViewModel IVM)
        //{
        //
        //   /* if (!ModelState.IsValid)
        //    {
        //        // Si le formulaire n'est pas valide, retournez à la vue avec les erreurs
        //        return View(IVM);
        //    }*/
        //
        //    // Créer une instance de Enrollment
        //    var enrollment = new Enrollment
        //    {
        //        EtudiantId = IVM.Etudiant.Id,
        //        CoursId = IVM.Cours.Id,
        //        Progression = 0, // Progression initiale
        //        IsConnected = false, // Par défaut, l'étudiant n'est pas connecté
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };
        //
        //    // Créer une instance de Payment
        //    var payment = new Payment
        //    {
        //        OwnerName = IVM.OwnerName,
        //        EtudiantId = IVM.Etudiant.Id,
        //        CoursId = IVM.Cours.Id,
        //        Amount = IVM.Cours.Price,
        //        PaymentDate = DateTime.UtcNow,
        //        CVC = IVM.Cvc,
        //        NumeroCarte = IVM.NumeroCarte,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow
        //    };
        //
        //    // Enregistrer les instances dans la base de données
        //    _context.Enrollments.Add(enrollment);
        //    _context.Payments.Add(payment);
        //    await _context.SaveChangesAsync();
        //
        //    // Rediriger vers une autre page (par exemple, une page de confirmation)
        //    return RedirectToAction("homePage", "MyHome");
        //}
        //

        [HttpGet]
        [Authorize(Roles = "Etudiant")]
        public async Task<IActionResult> InscriptionCours(int id)
        {
            // Vérifiez si le cours existe
            var cours = await _coursService.GetCoursByIdAsync(id);
            if (cours == null)
            {
                return NotFound("Le cours spécifié est introuvable.");
            }

            // Récupérez l'utilisateur connecté
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Vous devez être connecté pour vous inscrire à un cours.");
            }

            // Récupérez les informations de l'étudiant
            var etudiant = await _etudiantService.GetEtudiantByIdAsync(user.Id);
            if (etudiant == null)
            {
                return BadRequest("L'étudiant associé à cet utilisateur est introuvable.");
            }
            // Vérifiez si l'étudiant est déjà inscrit au cours
            var estDejaInscrit = await _enrollmentService.EstEtudiantInscritAsync(etudiant.Id, cours.Id);
            if (estDejaInscrit)
            {
                // Rediriger vers la page de suivi du cours
                return RedirectToAction("Index", "FollowCourse", new { id = cours.Id });
            }

            // Préparez le modèle pour la vue
            var viewModel = new InscriptionPageViewModel
            {
                Etudiant = etudiant,
                Cours = cours,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InscriptionCours(InscriptionPageViewModel IVM)
        {
            // Validation du modèle
            //if (!ModelState.IsValid)
            //{
            //    return View(IVM);
            //}

            // Vérifiez si l'étudiant est déjà inscrit au cours
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EtudiantId == IVM.Etudiant.Id && e.CoursId == IVM.Cours.Id);

            if (existingEnrollment != null)
            {
                ModelState.AddModelError(string.Empty, "Vous êtes déjà inscrit à ce cours.");
                return RedirectToAction("Index", "FollowCourse", new { id = IVM.Cours.Id });
            }

            // Créez une nouvelle instance de l'inscription
            var enrollment = new Enrollment
            {
                EtudiantId = IVM.Etudiant.Id,
                CoursId = IVM.Cours.Id,
                Progression = 0, // Progression initiale
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Créez une nouvelle instance de paiement
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

            // Enregistrez les données dans la base de données
            try
            {
                _context.Enrollments.Add(enrollment);
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Une erreur est survenue lors de l'inscription : {ex.Message}");
                return View(IVM);
            }

            // Redirigez vers une page de confirmation ou une autre page
            return RedirectToAction("Index","FollowCourse", new {id = IVM.Cours.Id});
        }

    }
}
