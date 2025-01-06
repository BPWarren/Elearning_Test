

using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

            // Récupérer l'inscription (Enrollment) de l'étudiant
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EtudiantId == user.Id && e.CoursId == id);

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

            // Récupérer toutes les leçons du cours
            var lessons = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId)
                .OrderBy(l => l.NumeroPage)
                .Select(l => new
                {
                    l.Id,
                    l.Titre,
                    l.Contenu,
                    l.NumeroPage
                })
                .ToListAsync();

            // Si CurrentLeconId est null, attribuer la première leçon
            if (enrollment.CurrentLeconId == null && lessons.Any())
            {
                var firstLesson = lessons.First();
                enrollment.CurrentLeconId = firstLesson.Id;
                enrollment.Progression = (int)((firstLesson.NumeroPage / (double)lessons.Count) * 100);
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
            }

            // Récupérer la leçon actuelle
            var currentLecon = lessons.FirstOrDefault(l => l.Id == enrollment.CurrentLeconId);

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

        // Passer à une leçon spécifique
        public async Task<IActionResult> GoToLesson(int enrollmentId, int lessonId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            // Récupérer la leçon spécifique
            var lesson = await _context.Lecons
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                return NotFound("Leçon non trouvée.");
            }

            // Mettre à jour la leçon actuelle et la progression
            enrollment.CurrentLeconId = lesson.Id;
            enrollment.Progression = (int)((lesson.NumeroPage / (double)_context.Lecons.Count(l => l.CoursId == enrollment.CoursId)) * 100);
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }

        // Passer à la leçon suivante
        public async Task<IActionResult> NextLesson(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            // Récupérer la leçon actuelle
            var currentLecon = await _context.Lecons
                .FirstOrDefaultAsync(l => l.Id == enrollment.CurrentLeconId);

            if (currentLecon == null)
            {
                return NotFound("Leçon actuelle non trouvée.");
            }

            // Récupérer la leçon suivante
            var nextLesson = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId && l.NumeroPage > currentLecon.NumeroPage)
                .OrderBy(l => l.NumeroPage)
                .FirstOrDefaultAsync();

            if (nextLesson != null)
            {
                // Mettre à jour la leçon actuelle et la progression
                enrollment.CurrentLeconId = nextLesson.Id;
                enrollment.Progression = (int)((nextLesson.NumeroPage / (double)_context.Lecons.Count(l => l.CoursId == enrollment.CoursId)) * 100);
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Si c'est la dernière leçon, marquer le cours comme terminé
                enrollment.IsCompleted = true;
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }

        // Revenir à la leçon précédente
        public async Task<IActionResult> PreviousLesson(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            // Récupérer la leçon actuelle
            var currentLecon = await _context.Lecons
                .FirstOrDefaultAsync(l => l.Id == enrollment.CurrentLeconId);

            if (currentLecon == null)
            {
                return NotFound("Leçon actuelle non trouvée.");
            }

            // Récupérer la leçon précédente
            var previousLesson = await _context.Lecons
                .Where(l => l.CoursId == enrollment.CoursId && l.NumeroPage < currentLecon.NumeroPage)
                .OrderByDescending(l => l.NumeroPage)
                .FirstOrDefaultAsync();

            if (previousLesson != null)
            {
                // Mettre à jour la leçon actuelle et la progression
                enrollment.CurrentLeconId = previousLesson.Id;
                enrollment.Progression = (int)((previousLesson.NumeroPage / (double)_context.Lecons.Count(l => l.CoursId == enrollment.CoursId)) * 100);
                _context.Enrollments.Update(enrollment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }

        // Marquer le cours comme terminé
        public async Task<IActionResult> MarkAsCompleted(int enrollmentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);

            if (enrollment == null)
            {
                return NotFound("Inscription non trouvée.");
            }

            enrollment.IsCompleted = true;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { id = enrollment.CoursId });
        }
    }
}