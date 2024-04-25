using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using GreenwichUniversityMagazine.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Models.ViewModel;
using System.Text;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Authorization;
namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;



        public IActionResult Index(string? searchString, int? magazineid, int? termid, int? facultyid)
        {
            HomeVM homeVM = new HomeVM();
            DateTime currentDateTime = DateTime.Now;
            homeVM.Terms = _unitOfWork.TermRepository.GetAll().Where(t => t.StartDate <= currentDateTime).ToList();
            homeVM.Facultys = _unitOfWork.FacultyRepository.GetAll().ToList();
            homeVM.Magazines = _unitOfWork.MagazineRepository.GetAll().Where(t => t.StartDate <= currentDateTime).ToList();
            homeVM.Articles = _unitOfWork.ArticleRepository.GetAll().Where(a => a.Status == true).OrderByDescending(a => a.ArticleId).ToList();
            return View(homeVM);
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
        public IActionResult Contact()
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
            User user = _unitOfWork.UserRepository.Login(email, HashPassword(password));
            if (user != null && user.Password == HashPassword(password))
            {
                if (user.Password == HashPassword(password))
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserRole", user.Role);
                    if (user.Name != null && user.avtUrl != null)
                    {
                        HttpContext.Session.SetString("UserName", user.Name);
                        HttpContext.Session.SetString("avtUrl", user.avtUrl);
                    }

                    //TempData["success"] = "Login successful"; // Lưu thông báo đăng nhập thành công vào TempData
                }
                return RedirectToAction("Index", "Home", new { area = "student" });
            }
            else
            {
                TempData["error"] = "Invalid email or password"; // Lưu thông báo lỗi vào TempData
            
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
            HttpContext.Session.Remove("UserRole");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("avtUrl");
            HttpContext.Session.Remove("UserEmail");
            return RedirectToAction("Index", "Home", new { area = "student" });
        }

        public IActionResult RedectNotification(int id)
        {
            //get current UserID
            //get all notification from DB
            //  List<Notification> notifications = _unitOfWork.NotificationRepository.GetAll().ToList();
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int userIdCurrent);
            User user = _unitOfWork.UserRepository.GetById(userIdCurrent);
            Comment comment = _unitOfWork.CommentRepository.Get(u => u.Id == id);
            if (comment != null)
            {
                comment.IsNotification = false;
                _unitOfWork.CommentRepository.Update(comment);
                _unitOfWork.Save();
                if (comment.Type.ToLower() == "private" && user.Role.ToLower() == "coordinate")
                {
                    return RedirectToAction("Detail", "Coordinate", new { area = "Coordinate", id = comment.ArticleId });
                }
                else if (comment.Type.ToLower() == "public" && user.Role.ToLower() == "coordinate")
                {
                    return RedirectToAction("SelectArticle", "Article", new { area = "Student", id = comment.ArticleId });
                }
                else if (comment.Type.ToLower() == "private" && user.Role.ToLower() == "student")
                {
                    return RedirectToAction("Update", "Article", new { area = "Student", id = comment.ArticleId });
                }
                else if (comment.Type.ToLower() == "public" && user.Role.ToLower() == "student")
                {
                    return RedirectToAction("SelectArticle", "Article", new { area = "Student", id = comment.ArticleId });
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetCommentsCount()
        {
            //Get information
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int userIdCurrent);
            if (userIdCurrent != 0)
            {
                HomeVM homeVM = new HomeVM();
                List<User> users = _unitOfWork.UserRepository.GetAll().ToList();
                homeVM.Articles = _unitOfWork.ArticleRepository.GetAll().Where(a => a.Status == true).OrderByDescending(a => a.ArticleId).ToList();
                homeVM.Facultys = _unitOfWork.FacultyRepository.GetAll().ToList();
                User user = _unitOfWork.UserRepository.GetById(userIdCurrent);
                int commentsCount;
                if (user.Role.ToLower() == "student")
                {
                    commentsCount = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.Article.UserId == userIdCurrent && u.UserId != userIdCurrent).ToList().Count();
                }
                else
                {
                    commentsCount = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.UserId != userIdCurrent && u.User.Role.ToLower() != "coordinate" && u.Type.ToLower() == "private" && u.Article.User.FacultyId == user.FacultyId).ToList().Count();
                }
                return Json(new { notifies = commentsCount });
            }
            else
                return Json(new { notifies = 0 });
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Create a new StringBuilder to collect the bytes
                // and create a string.
                StringBuilder stringBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    stringBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return stringBuilder.ToString();
            }
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetNotification()
        {
            //get current UserID
            //get all notification from DB
            //  List<Notification> notifications = _unitOfWork.NotificationRepository.GetAll().ToList();
            var notifications = new List<Comment>();
            List<User> users = _unitOfWork.UserRepository.GetAll().ToList();
            List<Article> article = _unitOfWork.ArticleRepository.GetAll().ToList();
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int userIdCurrent);

            User user = _unitOfWork.UserRepository.GetById(userIdCurrent);
            Faculty faculty = _unitOfWork.FacultyRepository.Get(u => u.Id == user.FacultyId);
            List<Comment> comments;
            if (user.Role.ToLower() == "student")
            {
                comments = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.Article.UserId == userIdCurrent && u.UserId != userIdCurrent).ToList();
            }
            else
            {
                comments = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.UserId != userIdCurrent && u.User.Role.ToLower() != "coordinate" && u.Type.ToLower() == "private" && u.Article.User.FacultyId == user.FacultyId).ToList();
            }
            // Return partial view with the data
            comments.Reverse();
            return PartialView("_Notification", comments);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
