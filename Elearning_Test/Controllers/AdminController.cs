using Elearning_Test.Data;
using Elearning_Test.Filters;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Elearning_Test.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel
            {
                // Statistiques principales
                TotalEtudiants = _context.Etudiants.Count(),
                TotalProfesseurs = _context.Professeurs.Count(),
                TotalCertificatsValides = _context.Certifications.Count(c => c.Validate),
                TotalCertificatsEnAttente = _context.Certifications.Count(c => !c.Validate),
                TotalCours = _context.Cours.Count(),
                TotalInscriptions = _context.Enrollments.Count(),
                CertifEnAttente = _context.Certifications.Where( c => !c.Validate).ToList(),
                CertifValide = _context.Certifications.Where(c => c.Validate).ToList(),
                // Statistiques d'activité
                ParticipantsActifs = _context.Etudiants.Count(e => e.LastLogin >= DateTime.UtcNow.AddDays(-30)),
                FormateursActifs = _context.Professeurs.Count(p => p.LastLogin >= DateTime.UtcNow.AddDays(-30)),
                // Graphiques
                InscriptionsParMois = GetInscriptionsParMois(),
                //VisiteursParMois = GetVisiteursParMois(),

                // Listes
                NouveauxEtudiants = _context.Etudiants
                    .OrderByDescending(e => e.CreatedAt)
                    .Take(5)
                    .ToList(),
                NouveauxCours = _context.Cours
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .ToList(),

                // Autres éléments essentiels
                //TotalVisiteurs = _context.Visitors.Count(),
                TotalPaiements = _context.Payments.Count(),
                RevenuTotal = _context.Payments.Sum(p => p.Amount),
                CoursPopulaires = GetCoursPopulaires(),

                // Utilisateurs en ligne
                EtudiantsEnLigne = _context.Etudiants
        .Where(e => e.IsConnected)
        .Select(e => new UserInfo
        {
            Nom = e.Nom,
            Prenom = e.Prenom,
            Email = e.Email
        })
        .ToList(),
                ProfesseursEnLigne = _context.Professeurs
        .Where(p => p.IsConnected)
        .Select(p => new UserInfo
        {
            Nom = p.Nom,
            Prenom = p.Prenom,
            Email = p.Email
        })
        .ToList()

            };

            return View(viewModel);
        }

        // Méthode pour récupérer les inscriptions par mois
        private Dictionary<string, int> GetInscriptionsParMois()
        {
            var inscriptionsParMois = _context.Etudiants
                .GroupBy(e => new { e.CreatedAt.Year, e.CreatedAt.Month })
                .Select(g => new
                {
                    Mois = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MM/yyyy"),
                    NombreInscriptions = g.Count()
                })
                .ToDictionary(x => x.Mois, x => x.NombreInscriptions);

            return inscriptionsParMois;
        }

        private Dictionary<string, int> GetCoursPopulaires()
        {
            var coursPopulaires = _context.Enrollments
                .GroupBy(e => e.Cours.Titre)
                .Select(g => new
                {
                    NomCours = g.Key,
                    NombreEtudiants = g.Count()
                })
                .OrderByDescending(x => x.NombreEtudiants)
                .Take(5) // Limiter aux 5 cours les plus populaires
                .ToDictionary(x => x.NomCours, x => x.NombreEtudiants);

            return coursPopulaires;
        }

        [HttpPost]
        public IActionResult ValiderCertification(int certificationId)
        {
            var certification = _context.Certifications.Find(certificationId);
            if (certification != null)
            {
                certification.Validate = true;
                certification.UpdatedAt = DateTime.UtcNow;
                _context.SaveChanges();
            }

            return RedirectToAction("Index"); // Redirige vers le tableau de bord après validation
        }
        // Méthode pour récupérer les visiteurs par mois
        //private Dictionary<string, int> GetVisiteursParMois()
        //{
        //    var visiteursParMois = _context.Visitors
        //        .GroupBy(v => new { v.VisitDate.Year, v.VisitDate.Month })
        //        .Select(g => new
        //        {
        //            Mois = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MM/yyyy"),
        //            NombreVisiteurs = g.Count()
        //        })
        //        .ToDictionary(x => x.Mois, x => x.NombreVisiteurs);

        //    return visiteursParMois;
        //}

        // Afficher la liste des admins
        public async Task<IActionResult> AllAdmin()
        {
            var admins = await _context.Admins.ToListAsync();
            return View(admins);
        }

        // Afficher la page d'ajout d'un nouvel admin
        public IActionResult Create()
        {
            return View();
        }

        // Ajouter un nouvel admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(RegisterAdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Si le modèle n'est pas valide, on retourne la vue avec les erreurs
                return View(model);
            }

            // Création de l'utilisateur admin
            var user = new Admin
            {
                UserName = model.Email,
                Nom = model.Nom,
                Email = model.Email,
                EmailConfirmed = true,
                IsConnected = false,  // L'utilisateur n'est pas connecté au début
                CreatedAt = DateTime.UtcNow,  // Date de création
                UpdatedAt = DateTime.UtcNow   // Date de mise à jour initiale
            };

            // Créer l'utilisateur avec un mot de passe
            var result = await _userManager.CreateAsync(user, model.MotDePasse);
            if (result.Succeeded)
            {
                // Assigner le rôle "Admin" à l'utilisateur
                await _userManager.AddToRoleAsync(user, "Admin");

                // Rediriger vers la page "AllAdmin"
                return RedirectToAction("AllAdmin", "Admin");
            }

            // En cas d'erreur, on les ajoute au ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }



        // Afficher la page de modification d'un admin
        public async Task<IActionResult> Edit(string userId)
        {
            var admin = await _context.Admins.FindAsync(userId); // Trouver l'administrateur par son ID (userId)
            if (admin == null)
            {
                return NotFound(); // Si l'administrateur n'est pas trouvé
            }

            // Initialiser le modèle de l'administrateur avec les données actuelles
            var model = new RegisterAdminViewModel
            {
                Nom = admin.Nom,
                Email = admin.Email,
                MotDePasse = "..",
                ConfirmationMotDePasse = "",
            };
            
            return View(model); // Retourner la vue avec le modèle
        }


        // Modifier un admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    admin.UpdatedAt = DateTime.UtcNow;
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(AllAdmin));
            }

            return View(admin);
        }

        // Afficher la page de confirmation de suppression
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // Supprimer un admin
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllAdmin));
        }

        // Vérifier si un admin existe
        private bool AdminExists(string id)
        {
            return _context.Admins.Any(e => e.Id == id);
        }

        // Actions supplémentaires (si nécessaires)
        public IActionResult SidebarStyle2()
        {
            return View();
        }

        public IActionResult Buttons()
        {
            return View("Components/Buttons");
        }
    }
}