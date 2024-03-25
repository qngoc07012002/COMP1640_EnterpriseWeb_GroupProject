using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Cordinate.Controllers
{
    [Area("cordinate")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Cordinate/Views/Home/Index.cshtml");
        }
    }
}
