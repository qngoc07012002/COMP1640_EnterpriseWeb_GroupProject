using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Guest.Controllers
{
    [Area("guest")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View("~/Areas/Guest/Views/Home/Index.cshtml");
        }
    }
}