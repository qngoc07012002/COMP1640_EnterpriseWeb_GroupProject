using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;

        public IActionResult Index()
        {
            return View("~/Areas/Student/Views/Home/Index.cshtml");
        }
        public HomeController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            User user = _unitOfWork.UserRepository.Login(email, password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("UserId", user.Id.ToString());
                TempData["success"] = "Login successful"; // Lưu thông báo đăng nhập thành công vào TempData
                return RedirectToAction("Index", "Home", new { area = "student" });
            }
            else
            {
                TempData["error"] = "Invalid email or password"; // Lưu thông báo lỗi vào TempData
                return View();
            }
        }



        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            
            if (_unitOfWork.UserRepository.CheckEmail(user) == false)
            {
                TempData["error"] = "Email Already Used";
                return Register();
            }
            else
            {
                _unitOfWork.UserRepository.Register(user);
                TempData["success"] = "Account created succesfully";


                _unitOfWork.Save();
                return RedirectToAction("Login");
            }

        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            return RedirectToAction("Index", "Home", new { area = "student" });
        }
    }
}
