using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.MVC.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public RoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("UserRole");

            // If no role in session, user is not logged in
            if (string.IsNullOrEmpty(userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // If user is logged in but has the wrong role
            if (userRole != _requiredRole)
            {
                // Redirect them to an Access Denied page or their own dashboard
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }

            base.OnActionExecuting(context);
        }
    }
}