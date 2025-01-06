using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearning_Test.Controllers
{
    public class FollowCourseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FollowCourseController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Afficher la leçon actuelle
        public async Task<IActionResult> Index(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            // Récupérer l'inscription (Enrollment) de l'étudiant avec les ID seulement
            var enrollment = await _context.Enrollments
                .Where(e => e.EtudiantId == user.Id && e.CoursId == id)
                .Select(e => new
                {
                    e.Id,
                    e.CoursId,
                    e.CurrentLeconId,
                    e.Progression,
                    e.IsCompleted
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return NotFound("Vous n'êtes pas inscrit à ce cours.");
            }

            // Récupérer les détails du cours en utilisant l'ID
            var cours = await _context.Cours
                .Where(c => c.Id == enrollment.CoursId)
                .Select(c => new
                {
                    c.Titre,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (cours == null)
            {
                return NotFound("Les détails du cours sont introuvables.");
            }

            // Récupérer les détails de la leçon actuelle en utilisant l'ID
            var currentLecon = await _context.Lecons
                .Where(l => l.Id == enrollment.CurrentLeconId)
                .Select(l => new
                {
                    l.Id,
                    l.Titre,
                    l.Contenu,
                    l.NumeroPage
                })
                .FirstOrDefaultAsync();

            // Récupérer la liste des leçons du cours
            var lessons = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId)
                .OrderBy(l => l.NumeroPage)
                .Select(l => new
                {
                    l.Id,
                    l.Titre,
                    l.NumeroPage
                })
                .ToListAsync();

            // Passer les données à la vue
            ViewBag.CourseTitle = cours.Titre;
            ViewBag.CourseDescription = cours.Description;
            ViewBag.CurrentLesson = currentLecon;
            ViewBag.Lessons = lessons;
            ViewBag.EnrollmentId = enrollment.Id;
            ViewBag.Progression = enrollment.Progression;
            ViewBag.IsCompleted = enrollment.IsCompleted;

            return View();
        }

        // Passer à la leçon suivante
        public async Task<IActionResult> NextLesson(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .Where(e => e.Id == enrollmentId)
                .Select(e => new
                {
                    e.Id,
                    e.CoursId,
                    e.CurrentLeconId,
                    e.Progression,
                    e.IsCompleted
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            // Récupérer la leçon actuelle
            var currentLecon = await _context.Lecons
                .Where(l => l.Id == enrollment.CurrentLeconId)
                .Select(l => new
                {
                    l.NumeroPage
                })
                .FirstOrDefaultAsync();

            if (currentLecon == null)
            {
                return NotFound("Leçon actuelle non trouvée.");
            }

            // Récupérer la leçon suivante
            var nextLesson = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId && l.NumeroPage > currentLecon.NumeroPage)
                .OrderBy(l => l.NumeroPage)
                .Select(l => new
                {
                    l.Id,
                    l.NumeroPage
                })
                .FirstOrDefaultAsync();

            if (nextLesson != null)
            {
                // Mettre à jour la leçon actuelle et la progression
                await _context.Enrollments
                    .Where(e => e.Id == enrollmentId)
                    .ExecuteUpdateAsync(e => e
                        .SetProperty(x => x.CurrentLeconId, nextLesson.Id)
                        .SetProperty(x => x.Progression, (int)((nextLesson.NumeroPage / (double)_context.Lecons.Count(l => l.CoursId == enrollment.CoursId)) * 100))
                    );
            }
            else
            {
                // Si c'est la dernière leçon, marquer le cours comme terminé
                await _context.Enrollments
                    .Where(e => e.Id == enrollmentId)
                    .ExecuteUpdateAsync(e => e
                        .SetProperty(x => x.IsCompleted, true)
                    );
            }

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }

        // Revenir à la leçon précédente
        public async Task<IActionResult> PreviousLesson(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .Where(e => e.Id == enrollmentId)
                .Select(e => new
                {
                    e.Id,
                    e.CoursId,
                    e.CurrentLeconId,
                    e.Progression,
                    e.IsCompleted
                })
                .FirstOrDefaultAsync();

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            // Récupérer la leçon actuelle
            var currentLecon = await _context.Lecons
                .Where(l => l.Id == enrollment.CurrentLeconId)
                .Select(l => new
                {
                    l.NumeroPage
                })
                .FirstOrDefaultAsync();

            if (currentLecon == null)
            {
                return NotFound("Leçon actuelle non trouvée.");
            }

            // Récupérer la leçon précédente
            var previousLesson = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId && l.NumeroPage < currentLecon.NumeroPage)
                .OrderByDescending(l => l.NumeroPage)
                .Select(l => new
                {
                    l.Id,
                    l.NumeroPage
                })
                .FirstOrDefaultAsync();

            if (previousLesson != null)
            {
                // Mettre à jour la leçon actuelle et la progression
                await _context.Enrollments
                    .Where(e => e.Id == enrollmentId)
                    .ExecuteUpdateAsync(e => e
                        .SetProperty(x => x.CurrentLeconId, previousLesson.Id)
                        .SetProperty(x => x.Progression, (int)((previousLesson.NumeroPage / (double)_context.Lecons.Count(l => l.CoursId == enrollment.CoursId)) * 100))
                    );
            }

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }

        // Marquer le cours comme terminé
        public async Task<IActionResult> MarkAsCompleted(int enrollmentId)
        {
            await _context.Enrollments
                .Where(e => e.Id == enrollmentId)
                .ExecuteUpdateAsync(e => e
                    .SetProperty(x => x.IsCompleted, true)
                );

            return RedirectToAction("Index", new { id = _context.Enrollments.FirstOrDefault(e => e.Id == enrollmentId)?.CoursId });
        }
    }
}