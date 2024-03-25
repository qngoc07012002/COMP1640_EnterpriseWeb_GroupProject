using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GreenwichUniversityMagazine.Authentication
{
    public class AdminAuthentication : ActionFilterAttribute
    {
        public AdminAuthentication() { }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRoles = context.HttpContext.Session.GetString("UserRole");
            if (userRoles == null || !userRoles.Contains("ADMIN"))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    area = "Student",
                    controller = "Home",
                    action = "Index",
                }));
            }
        }

    }
}
