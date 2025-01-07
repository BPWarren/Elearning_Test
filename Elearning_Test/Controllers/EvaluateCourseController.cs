
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Elearning_Test.Controllers
{
    public class EvaluateCourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EvaluateCourseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Afficher le formulaire d'évaluation pour un cours spécifique
        [HttpGet]
        public async Task<IActionResult> Index(int id)
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

            // Récupérer les détails du cours
            var cours = await _context.Cours
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cours == null)
            {
                return NotFound("Cours non trouvé.");
            }

            // Passer les données du cours à la vue
            ViewBag.CoursId = id;
            ViewBag.CoursTitre = cours.Titre;

            return View();
        }

        // Traiter la soumission du formulaire d'évaluation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(int id, [Bind("Content")] Evaluation evaluation)
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

            
                // Créer une nouvelle évaluation
            var newEvaluation = new Evaluation
            {
                Content = evaluation.Content,
                CoursId = id,
                EtudiantId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Ajouter l'évaluation à la base de données
            _context.Evaluations.Add(newEvaluation);
            await _context.SaveChangesAsync();

            // Rediriger vers une page de confirmation ou le tableau de bord
            return RedirectToAction("Index", "DashboardEtudiant");
        }
    }
}