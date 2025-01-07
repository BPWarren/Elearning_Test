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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Récupérer les cours de l'étudiant
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

            // Récupérer les certificats validés de l'étudiant
            var certificats = await _context.Certifications
                .Where(c => c.EtudiantId == userId && c.Validate)
                .Select(c => new CertificatViewModel
                {
                    CertificationId = c.Id,
                    CoursId = c.CoursId,
                    CoursTitre = c.Cours.Titre,
                    DateValidation = c.UpdatedAt
                })
                .ToListAsync();

            // Passer les données à la vue
            var viewModel = new DashboardEtudiantViewModel
            {
                CoursEnCours = coursEnCours,
                Certificats = certificats
            };

            return View(viewModel);
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

        public async Task<IActionResult> DemanderCertificat(int id)
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

            // Vérifier si une demande de certificat existe déjà
            var existingCertification = await _context.Certifications
                .FirstOrDefaultAsync(c => c.EtudiantId == userId && c.CoursId == id);

            if (existingCertification != null)
            {
                return BadRequest("Vous avez déjà demandé un certificat pour ce cours.");
            }

            // Créer une nouvelle demande de certificat
            var certification = new Certification
            {
                EtudiantId = userId,
                CoursId = id,
                Validate = false, // Le certificat n'est pas encore validé par l'admin
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Certifications.Add(certification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Rediriger vers le tableau de bord
        }

        public async Task<IActionResult> TelechargerCertificat(int id)
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Vérifier si le certificat existe et est validé
            var certification = await _context.Certifications
                .FirstOrDefaultAsync(c => c.Id == id && c.EtudiantId == userId && c.Validate);

            if (certification == null)
            {
                return NotFound("Certificat non trouvé ou non validé.");
            }

            // Récupérer les informations du cours associé au certificat
            var cours = await _context.Cours
                .FirstOrDefaultAsync(c => c.Id == certification.CoursId);

            if (cours == null)
            {
                return NotFound("Cours associé au certificat non trouvé.");
            }

            // Générer le certificat (par exemple, un PDF)
            var certificatContent = GenererCertificat(certification, cours);

            // Retourner le fichier à télécharger
            return File(certificatContent, "application/pdf", $"Certificat_{cours.Titre}.pdf");
        }

        private byte[] GenererCertificat(Certification certification, Cours cours)
        {
            // Ici, vous pouvez utiliser une bibliothèque comme iTextSharp ou QuestPDF pour générer un PDF
            // Ceci est un exemple simplifié
            var certificatText = $"Certificat de réussite\n\n" +
                                 $"Étudiant: {certification.EtudiantId}\n" + // Vous pouvez récupérer le nom de l'étudiant si nécessaire
                                 $"Cours: {cours.Titre}\n" +
                                 $"Date de validation: {certification.UpdatedAt.ToShortDateString()}\n";

            return System.Text.Encoding.UTF8.GetBytes(certificatText);
        }

        public async Task<IActionResult> Evaluations()
        {
            // Récupérer l'ID de l'étudiant connecté
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Récupérer les évaluations de l'étudiant
            var evaluations = await _context.Evaluations
                .Where(e => e.EtudiantId == userId)
                .Join(_context.Cours,
                    evaluation => evaluation.CoursId,
                    cours => cours.Id,
                    (evaluation, cours) => new EvaluationViewModel
                    {
                        Id = evaluation.Id,
                        CoursTitre = cours.Titre,
                        Contenu = evaluation.Content,
                        DateEvaluation = evaluation.CreatedAt
                    })
                .ToListAsync();

            // Passer les données à la vue
            return View(evaluations);
        }



    }
}