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
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.IsInRoleAsync(user, "admin"))
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("AllAdmin", "Admin");
                }
            }

            ModelState.AddModelError("", "Identifiants incorrects ou rôle non autorisé.");
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
            if (!ModelState.IsValid) return View(model);

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
