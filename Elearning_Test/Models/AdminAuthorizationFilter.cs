using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Elearning_Test.Models; // Assurez-vous que le modèle Admin est ici

namespace Elearning_Test.Filters
{
    public class AdminAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly UserManager<Admin> _userManager;

        // Injecter UserManager dans le filtre
        public AdminAuthorizationFilter(UserManager<Admin> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (user == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("AdminLogin", "Account", null); // Rediriger vers la page de connexion si non authentifié
                return;
            }

            // Utilisation de `await` pour obtenir l'utilisateur de manière asynchrone
            var admin = await _userManager.GetUserAsync(user);

            if (admin == null || !await _userManager.IsInRoleAsync(admin, "Admin")) // Vérifier si l'utilisateur est bien un administrateur
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null); // Rediriger vers AccessDenied si l'utilisateur n'a pas le rôle Admin
            }
        }
    }
}