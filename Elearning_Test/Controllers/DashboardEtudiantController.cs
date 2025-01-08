using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using iText.IO.Font;
using iText.Kernel.Font;
using System.Drawing;
using System.IO;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout.Borders;

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

            // Récupérer les certificats de l'étudiant
            var certificats = await _context.Certifications
                .Where(c => c.EtudiantId == userId)
                .Select(c => new CertificatViewModel
                {
                    CertificationId = c.Id,
                    CoursId = c.CoursId,
                    CoursTitre = c.Cours.Titre,
                    DateValidation = c.UpdatedAt,
                    EstEnAttente = !c.Validate // Certificat en attente si non validé
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

            // Récupérer les informations de l'étudiant
            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Id == certification.EtudiantId);

            if (etudiant == null)
            {
                return NotFound("Étudiant associé au certificat non trouvé.");
            }

            // Générer le certificat (par exemple, un PDF)
            var certificatContent = GenererCertificat(certification, cours, etudiant);

            // Retourner le fichier à télécharger
            return File(certificatContent, "application/pdf", $"Certificat_{cours.Titre}.pdf");
        }





        private byte[] GenererCertificat(Certification certification, Cours cours, Etudiant etudiant)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Créer un document PDF
                var writer = new PdfWriter(memoryStream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // Définir un fond blanc
                document.SetBackgroundColor(iText.Kernel.Colors.ColorConstants.WHITE);

                // Charger les polices personnalisées (remplacez par vos polices si nécessaire)
                var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "Roboto-Regular.ttf");
                var boldFontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "Roboto-Bold.ttf");
                var italicFontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "Roboto-Italic.ttf");

                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                var boldFont = PdfFontFactory.CreateFont(boldFontPath, PdfEncodings.IDENTITY_H);
                var italicFont = PdfFontFactory.CreateFont(italicFontPath, PdfEncodings.IDENTITY_H);

                // Titre du certificat
                document.Add(new Paragraph("CERTIFICAT DE RÉUSSITE")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(32)
                    .SetFont(boldFont)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
                    .SetMarginTop(40));

                document.Add(new Paragraph("\n"));

                // Ligne de séparation décorative
                var line = new LineSeparator(new SolidLine(1f))
                    .SetWidth(300)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(line);

                // Texte principal
                document.Add(new Paragraph("Ce certificat est décerné à")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetFont(italicFont)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK));

                document.Add(new Paragraph($"{etudiant.Nom} {etudiant.Prenom}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(28)
                    .SetFont(boldFont)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
                    .SetMarginBottom(20));

                document.Add(new Paragraph("pour avoir démontré une excellence académique")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetFont(italicFont)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK));

                document.Add(new Paragraph($"dans le cours : {cours.Titre}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetFont(italicFont)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK));

                document.Add(new Paragraph($"Date de validation : {certification.UpdatedAt.ToShortDateString()}")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(18)
                    .SetFont(font)
                    .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)
                    .SetMarginBottom(40));

                // Ligne de séparation décorative
                var line2 = new LineSeparator(new SolidLine(1f))
                    .SetWidth(300)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(line2);

                // Ajouter un badge ou un sceau en bas du certificat
                var sealPath = Path.Combine(Directory.GetCurrentDirectory(), "CertifImages", "gold-seal.png");
                if (System.IO.File.Exists(sealPath))
                {
                    var seal = new iText.Layout.Element.Image(iText.IO.Image.ImageDataFactory.Create(sealPath))
                        .SetWidth(120) // Taille du sceau
                        .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                        .SetMarginBottom(20); // Espacement en bas
                    document.Add(seal);
                }

                // Section des signatures (Doyen à gauche, Proviseur à droite)
                var table = new Table(2) // 2 colonnes
                    .UseAllAvailableWidth()
                    .SetMarginTop(20)
                    .SetBorder(Border.NO_BORDER); // Supprimer les bordures du tableau

                // Colonne de gauche : Doyen
                table.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER) // Supprimer les bordures de la cellule
                    .SetTextAlignment(TextAlignment.LEFT)
                    .Add(new Paragraph("GABA Kossi Martinien")
                        .SetFontSize(16)
                        .SetFont(font)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK))
                    .Add(new Paragraph("Doyen de l'université")
                        .SetFontSize(14)
                        .SetFont(italicFont)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)));

                // Colonne de droite : Proviseur
                table.AddCell(new Cell()
                    .SetBorder(Border.NO_BORDER) // Supprimer les bordures de la cellule
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .Add(new Paragraph("QUIDAH Deserema Espoir")
                        .SetFontSize(16)
                        .SetFont(font)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK))
                    .Add(new Paragraph("Proviseure")
                        .SetFontSize(14)
                        .SetFont(italicFont)
                        .SetFontColor(iText.Kernel.Colors.ColorConstants.BLACK)));

                document.Add(table);

                // Fermer le document
                document.Close();

                // Retourner le contenu du PDF sous forme de tableau de bytes
                return memoryStream.ToArray();
            }
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