using GreenwichUniversityMagazine.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("manager")]
    [ManagerAuthentication()]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Areas/Manager/Views/Home/Index.cshtml");
        }
    }
}
