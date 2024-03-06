using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Guest.Controllers
{
    [Area("guest")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Guest/Views/Home/Index.cshtml");
        }
    }
}
