using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Elearning_Test.Controllers
{
    [Authorize(Roles = "Admin")] // Accessible uniquement aux administrateurs
    public class ProfesseurController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfesseurController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Afficher la liste des professeurs
        public async Task<IActionResult> AllProfesseurs()
        {
            var professeurs = await _context.Professeurs.ToListAsync();
            return View(professeurs);
        }

        // Afficher la page d'ajout d'un nouveau professeur
        public IActionResult Create()
        {
            return View();
        }

        // Ajouter un nouveau professeur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfesseurRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Retourner la vue si les données ne sont pas valides
            }

            var professeur = new Professeur
            {
                UserName = model.Email,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Email = model.Email,
                Specialite = model.Specialite,
                IsConnected = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(professeur, model.MotDePasse);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(professeur, "Professeur");
                return RedirectToAction(nameof(AllProfesseurs));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Afficher la page de modification d'un professeur
        //public async Task<IActionResult> Edit(string id)
        //{
        //    var professeur = await _context.Professeurs.FindAsync(id);
        //    if (professeur == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new ProfesseurRegistrationViewModel
        //    {
        //        Nom = professeur.Nom,
        //        Prenom = professeur.Prenom,
        //        Email = professeur.Email,
        //        Specialite = professeur.Specialite
        //    };

        //    return View(model);
        //}

        // Modifier un professeur
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProfesseurRegistrationViewModel model)
        {
            var professeur = await _context.Professeurs.FindAsync(id);
            if (professeur == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            professeur.Nom = model.Nom;
            professeur.Prenom = model.Prenom;
            professeur.Email = model.Email;
            professeur.Specialite = model.Specialite;
            professeur.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(model.MotDePasse))
            {
                var passwordHasher = _userManager.PasswordHasher;
                professeur.PasswordHash = passwordHasher.HashPassword(professeur, model.MotDePasse);
            }

            try
            {
                _context.Update(professeur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllProfesseurs));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfesseurExists(professeur.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // Afficher la page de confirmation de suppression
        public async Task<IActionResult> Delete(string id)
        {
            var professeur = await _context.Professeurs.FirstOrDefaultAsync(m => m.Id == id);
            if (professeur == null)
            {
                return NotFound();
            }

            return View(professeur);
        }

        // Supprimer un professeur
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var professeur = await _context.Professeurs.FindAsync(id);
            if (professeur == null)
            {
                return NotFound();
            }

            _context.Professeurs.Remove(professeur);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AllProfesseurs));
        }

        // Vérifier si un professeur existe
        private bool ProfesseurExists(string id)
        {
            return _context.Professeurs.Any(e => e.Id == id);
        }
    }
}
