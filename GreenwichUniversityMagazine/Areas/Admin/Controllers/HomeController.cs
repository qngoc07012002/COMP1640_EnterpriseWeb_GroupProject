using GreenwichUniversityMagazine.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("admin")]
        [AdminAuthentication()]
        public IActionResult Index()
        {
            return View("~/Areas/Admin/Views/Home/Index.cshtml"); 
        }
    }
}
