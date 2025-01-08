// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Elearning_Test.Areas.Identity.Pages.Account
{

    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager; // Ajouter UserManager pour récupérer l'utilisateur
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApplicationDbContext _dbContext; // Ajouter DbContext pour accéder à la base de données

        public LogoutModel(SignInManager<IdentityUser> signInManager,
                           UserManager<IdentityUser> userManager,
                           ILogger<LogoutModel> logger,
                           ApplicationDbContext dbContext) // Injecter le DbContext
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            var user = await _userManager.GetUserAsync(User); // Récupérer l'utilisateur actuel
            if (user != null)
            {
                // Vérifier si l'utilisateur est un Professeur ou un Participant
                if (user is Professeur professeur)
                {
                    professeur.IsConnected = false; // Mettre à jour IsConnected
                    await _dbContext.SaveChangesAsync(); // Sauvegarder dans la base de données
                }
                else if (user is Etudiant participant)
                {
                    participant.IsConnected = false; // Mettre à jour IsConnected
                    await _dbContext.SaveChangesAsync(); // Sauvegarder dans la base de données
                }
                else if (user is Admin admin)
                {
                    admin.IsConnected = false; // Mettre à jour IsConnected
                    await _dbContext.SaveChangesAsync(); // Sauvegarder dans la base de données
                }
            }

            await _signInManager.SignOutAsync(); // Déconnexion de l'utilisateur
            _logger.LogInformation("User logged out.");

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl); // Rediriger vers l'URL de retour
            }
            else
            {
                return RedirectToPage(); // Rediriger vers la même page
            }
        }
    }

}
