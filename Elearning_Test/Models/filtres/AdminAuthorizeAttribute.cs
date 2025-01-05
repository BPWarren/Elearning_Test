using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AdminAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        // Vérifier si l'utilisateur est connecté
        if (!user.Identity.IsAuthenticated)
        {
            // Rediriger vers la page AdminLogin si non connecté
            context.Result = new RedirectToActionResult("AdminLogin", "Account", null);
            return;
        }

        // Vérifier si l'utilisateur a le rôle "Admin"
        if (!user.IsInRole("Admin"))
        {
            // Rediriger vers la page HomePage si l'utilisateur n'est pas administrateur
            context.Result = new RedirectToActionResult("HomePage", "Home", null);
        }
    }
}