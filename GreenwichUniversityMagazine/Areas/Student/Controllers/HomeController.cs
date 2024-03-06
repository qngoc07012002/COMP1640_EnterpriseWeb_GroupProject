using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Student/Views/Home/Index.cshtml");
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
