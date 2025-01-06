using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompleteRoles.Controllers
{

    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }

        //Admin login

        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AdminLogin(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Veuillez renseigner tous les champs.");
                return View();
            }

            // Récupérer l'utilisateur par email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Utilisateur non trouvé.");
                return View();
            }
            if (!await _userManager.IsInRoleAsync(user, "admin"))
            {
                ModelState.AddModelError("", "Rôle non autorisé.");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (user is Admin admin)
                {
                    admin.IsConnected = true; // Mettre à jour le statut
                    _dbContext.Admins.Update(admin); // Mettre à jour l'entité dans le contexte
                    await _dbContext.SaveChangesAsync(); // Sauvegarder dans la base de données
                }

                return RedirectToAction("AllAdmin", "Admin");
            }
            else
            {
                ModelState.AddModelError("", "Identifiants incorrects.");
            }

            return View();
        }

        //Login begins Here
        [HttpGet]
        public IActionResult Login(string role)
        {
            ViewData["Role"] = role;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string role, string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.IsInRoleAsync(user, role))
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (user is Professeur professeur)
                    {
                        professeur.IsConnected = true;  // Mise à jour du champ IsConnected
                        await _dbContext.SaveChangesAsync();  // Sauvegarde de la mise à jour
                    }
                    // Si l'utilisateur est un Participant
                    else if (user is Etudiant participant)
                    {
                        participant.IsConnected = true;  // Mise à jour du champ IsConnected
                        await _dbContext.SaveChangesAsync();  // Sauvegarde de la mise à jour
                    }

                    return RedirectToAction("homePage", "MyHome");
                }
            }
           
            ModelState.AddModelError("", "Identifiants incorrects ou rôle non autorisé.");
            return View();
        }

        //Registration Professeur begin here

        [HttpGet]
        public IActionResult RegisterProfesseur()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterProfesseur(ProfesseurRegistrationViewModel model)
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

            var user = new Professeur
            {
                UserName = model.UserName,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Specialite = model.Specialite,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.MotDePasse);
            if (result.Succeeded)
            {
                // Assigner le rôle
                await _userManager.AddToRoleAsync(user, "Professeur");

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("homePage", "MyHome");
            }

            // Gestion des erreurs lors de la création de l'utilisateur
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        //Registration Prof ends here

        //Registration Etudiant begins here

        [HttpGet]
        public IActionResult RegisterEtudiant()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEtudiant(EtudiantRegistrationViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

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
            var user = new Etudiant
            {
                UserName = model.UserName,
                Nom = model.Nom,
                Prenom = model.Prenom,
                Cne = model.Cne,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.MotDePasse);
            if (result.Succeeded)
            {
                // Assigner le rôle
                await _userManager.AddToRoleAsync(user, "Etudiant");


                return RedirectToAction("homePage", "MyHome");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
        //Registration Etduiant ends here

    }


}
