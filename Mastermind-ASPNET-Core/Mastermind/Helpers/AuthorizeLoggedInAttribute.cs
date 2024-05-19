using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Mastermind.Helpers
{
    //La logique de cette classe ne vient pas demoi...source: ChatGPT

    public class AuthorizeLoggedInAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;

            // Vérifie si l'utilisateur est authentifié
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                // Redirige vers la page de connexion si l'utilisateur n'est pas authentifié
                context.Result = new RedirectToActionResult("Login", "Auth", new { area = "" });
            }

            base.OnActionExecuting(context);
        }
    }
}
