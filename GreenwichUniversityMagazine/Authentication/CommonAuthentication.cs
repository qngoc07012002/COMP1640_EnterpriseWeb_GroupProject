using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GreenwichUniversityMagazine.Authentication
{
    public class CommonAuthentication : ActionFilterAttribute
    {
        public CommonAuthentication() { }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRoles = context.HttpContext.Session.GetString("UserRole");
            if (userRoles == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    area = "Student",
                    controller = "Home",
                    action = "Login",
                }));
            }
        }

    }
}
