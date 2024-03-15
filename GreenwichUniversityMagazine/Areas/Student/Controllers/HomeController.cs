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
            IEnumerable<Article> articleList = _unitOfWork.ArticleRepository.GetAll(includeProperty: "Magazines");
            return View(articleList);
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
            if (user != null&& user.Password == password)
            {
                if (user.Password == password)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    if(user.Name!=null && user.avtUrl != null)
                    {
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("avtUrl", user.avtUrl);
                    }
                  
                    /*TempData["success"] = "Login successful";*/ // Lưu thông báo đăng nhập thành công vào TempData
                }
                return RedirectToAction("Index", "Home", new { area = "student" });
            }
            else
            {
                TempData["error"] = "Invalid email or password"; // Lưu thông báo lỗi vào TempData
                ViewBag.Error = TempData["error"];
                return View("Login");
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
                ViewBag.Error = TempData["error"]; // Thiết lập ViewBag.Error với giá trị của TempData["error"]
                return View(user);
            }
            else
            {
                _unitOfWork.UserRepository.Register(user);
                TempData["success"] = "Account created successfully";
                ViewBag.Success = TempData["success"]; // Thiết lập ViewBag.Success với giá trị của TempData["success"]
                _unitOfWork.Save();
                return RedirectToAction("Login");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("avtUrl");
            HttpContext.Session.Remove("UserEmail");

            return RedirectToAction("Index", "Home", new { area = "student" });
        }
    }
}
