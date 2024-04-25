using GreenwichUniversityMagazine.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Cordinate.Controllers
{
    [Area("cordinate")]
    [CoordinateAuthentication()]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Cordinate/Views/Home/Index.cshtml");
        }
    }
}
