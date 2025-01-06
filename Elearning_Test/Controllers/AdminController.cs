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
                // Récupérer le nombre total d'étudiants
                TotalEtudiants = _context.Etudiants.Count(),

                // Récupérer le nombre total de professeurs
                TotalProfesseurs = _context.Professeurs.Count(),

                // Récupérer le nombre total de cours
                TotalCours = _context.Cours.Count(),

                // Récupérer le nombre total de certificats délivrés
                TotalCertificats = _context.Certifications.Count(),

                // Récupérer les inscriptions par mois
                InscriptionsParMois = GetInscriptionsParMois(),

                // Récupérer les cours les plus populaires
                CoursPopulaires = GetCoursPopulaires()
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

        // Méthode pour récupérer les cours les plus populaires
        private Dictionary<string, int> GetCoursPopulaires()
        {
            var coursPopulaires = _context.Enrollments
                .GroupBy(i => i.Cours.Titre)
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