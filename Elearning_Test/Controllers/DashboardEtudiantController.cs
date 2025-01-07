using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Elearning_Test.Controllers
{
    public class DashboardEtudiantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardEtudiantController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Afficher le tableau de bord de l'étudiant
        public async Task<IActionResult> Index()
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Récupérer les cours en cours et terminés de l'étudiant
            var coursEnCours = await _context.Enrollments
                .Where(e => e.EtudiantId == userId)
                .Join(_context.Cours,
                      enrollment => enrollment.CoursId,
                      cours => cours.Id,
                      (enrollment, cours) => new CoursViewModel2
                      {
                          Id = cours.Id,
                          Titre = cours.Titre,
                          Description = cours.Description,
                          Progression = enrollment.Progression,
                          EstTermine = enrollment.IsCompleted
                      })
                .ToListAsync();

            // Passer les données à la vue
            return View(coursEnCours);
        }

        // Continuer un cours
        public async Task<IActionResult> ContinuerCours(int id)
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vérifier si l'étudiant est inscrit à ce cours
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EtudiantId == userId && e.CoursId == id);

            if (enrollment == null)
            {
                return NotFound("Vous n'êtes pas inscrit à ce cours.");
            }

            // Rediriger vers la page de suivi du cours
            return RedirectToAction("Index", "FollowCourse", new { id = id });
        }

        // Afficher les statistiques de l'étudiant
        public async Task<IActionResult> Statistiques()
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Récupérer les inscriptions de l'étudiant
            var enrollments = await _context.Enrollments
                .Where(e => e.EtudiantId == userId)
                .ToListAsync();

            // Calculer les statistiques
            var statistiques = new StatistiquesEtudiantViewModel
            {
                NombreCoursInscrits = enrollments.Count,
                NombreCoursTermines = enrollments.Count(e => e.IsCompleted),
                ProgressionMoyenne = enrollments.Any() ? (int)enrollments.Average(e => e.Progression) : 0
            };

            // Passer les données à la vue
            return View(statistiques);
        }

        public async Task<IActionResult> EvaluerCours(int id)
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vérifier si l'étudiant est inscrit à ce cours et si le cours est terminé
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EtudiantId == userId && e.CoursId == id && e.IsCompleted);

            if (enrollment == null)
            {
                return NotFound("Vous n'êtes pas inscrit à ce cours ou le cours n'est pas terminé.");
            }

            // Rediriger vers la page d'évaluation du cours
            return RedirectToAction("Index", "EvaluateCourse", new { id = id });
        }
    }
}