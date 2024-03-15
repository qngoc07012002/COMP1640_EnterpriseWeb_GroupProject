using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GreenwichUniversityMagazine.Authentication
{
    public class CoordinateAuthentication : ActionFilterAttribute
    {
        public CoordinateAuthentication() { }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRoles = context.HttpContext.Session.GetString("UserRole");
            if (userRoles == null || !userRoles.Contains("COORDINATE"))
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
