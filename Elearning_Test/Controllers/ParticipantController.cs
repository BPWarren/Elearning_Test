using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Elearning_Test.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ParticipantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ParticipantController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Afficher la liste des étudiants
        public async Task<IActionResult> AllParticipants()
        {
            var etudiants = await _context.Etudiants.ToListAsync();
            return View(etudiants);
        }

        // Afficher la page d'ajout d'un nouvel étudiant
        public IActionResult Create()
        {
            return View();
        }

        // Ajouter un nouvel étudiant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EtudiantRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Vérification de l'unicité de l'email
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Cette adresse e-mail est déjà utilisée par un autre utilisateur.");
                return View(model);
            }

            var existingUsercne = await _userManager.FindByEmailAsync(model.Cne);
            if (existingUsercne != null)
            {
                ModelState.AddModelError("Cne", "Ce Cne est déjà utilisée par un autre utilisateur.");
                return View(model);
            }

            var etudiant = new Etudiant
            {
                UserName = model.UserName,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Cne = model.Cne,
                Email = model.Email,
                IsConnected = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(etudiant, model.MotDePasse);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(etudiant, "Etudiant");
                return RedirectToAction(nameof(AllParticipants));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Afficher la page de modification d'un étudiant
        //public async Task<IActionResult> Edit(string id)
        //{
        //    var etudiant = await _context.Etudiants.FindAsync(id);
        //    if (etudiant == null)
        //    {
        //        return NotFound();
        //    }

        //    var model = new EtudiantRegistrationViewModel
        //    {
        //        UserName = etudiant.UserName,
        //        Nom = etudiant.Nom,
        //        Prenom = etudiant.Prenom,
        //        Cne = etudiant.Cne,
        //        Email = etudiant.Email
        //    };

        //    return View(model);
        //}

        // Modifier un étudiant
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EtudiantRegistrationViewModel model)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            etudiant.UserName = model.UserName;
            etudiant.Nom = model.Nom;
            etudiant.Prenom = model.Prenom;
            etudiant.Cne = model.Cne;
            etudiant.Email = model.Email;
            etudiant.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrEmpty(model.MotDePasse))
            {
                var passwordHasher = _userManager.PasswordHasher;
                etudiant.PasswordHash = passwordHasher.HashPassword(etudiant, model.MotDePasse);
            }

            try
            {
                _context.Update(etudiant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(AllParticipants));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EtudiantExists(etudiant.Id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        // Afficher la page de confirmation de suppression d'un étudiant
        public async Task<IActionResult> Delete(string id)
        {
            var etudiant = await _context.Etudiants.FirstOrDefaultAsync(m => m.Id == id);
            if (etudiant == null)
            {
                return NotFound();
            }

            return View(etudiant);
        }

        // Supprimer un étudiant
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string Id)
        {
            var etudiant = await _context.Etudiants.FindAsync(Id);
            if (etudiant == null)
            {
                return NotFound();
            }
            _context.Etudiants.Remove(etudiant);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllParticipants));
        }

        // Vérifier si un étudiant existe
        private bool EtudiantExists(string id)
        {
            return _context.Etudiants.Any(e => e.Id == id);
        }
    }
}