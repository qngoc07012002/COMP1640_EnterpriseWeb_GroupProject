using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using GreenwichUniversityMagazine.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Models.ViewModel;
namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;




        /*public IActionResult Index(string searchString, int? id)
        {
            HomeVM homeVM = new HomeVM();

            if (id != null && id != 0)
            {
                homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyMagazine(id).ToList();
                homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyTerm(id).ToList();
                homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyFaculty(id).ToList();


                homeVM.Magazines = _unitOfWork.MagazineRepository.GetAllMagazine().ToList();
                homeVM.Terms = _unitOfWork.TermRepository.GetAllTerm().ToList();
                homeVM.Facultys = _unitOfWork.FacultyRepository.GetAllFaculty().ToList();
            }
            else
            {
                homeVM.Magazines = _unitOfWork.MagazineRepository.GetAllMagazine().ToList();
                homeVM.Terms = _unitOfWork.TermRepository.GetAllTerm().ToList();
                homeVM.Facultys = _unitOfWork.FacultyRepository.GetAllFaculty().ToList();

                if (!string.IsNullOrEmpty(searchString))
                {
                    homeVM.Articles = _unitOfWork.ArticleRepository.Search(searchString).ToList();
                }
                else
                {
                    homeVM.Articles = _unitOfWork.ArticleRepository.GetAll().ToList();
                }
            }

            return View(homeVM);
        }*/

        public IActionResult Index()
        {
            IEnumerable<Article> articleList = _unitOfWork.ArticleRepository.GetAll(includeProperty: "Magazines").ToList();
            //Get information
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int userIdCurrent);
            if (userIdCurrent != 0)
            {
                List<User> users = _unitOfWork.UserRepository.GetAll().ToList();

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
                ViewBag.CommentsCount = commentsCount;
            }

            return View(articleList);
        }




        public IActionResult SelectMagazine(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            HomeVM homeVM = new HomeVM();
           
            homeVM.Magazines = _unitOfWork.MagazineRepository.GetAllMagazine().ToList();
            homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyMagazine(id).ToList();
            
            return View("Index", homeVM);
        }

        public IActionResult SelectTerm(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            HomeVM homeVM = new HomeVM();
           
            homeVM.Magazines = _unitOfWork.TermRepository.GetAllTerm().ToList();
            homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyTerm(id).ToList();
            
            return View("Index", homeVM);
        }

        public IActionResult SelectFaculty(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            HomeVM homeVM = new HomeVM();
            
            homeVM.Magazines = _unitOfWork.FacultyRepository.GetAllFaculty().ToList();
            homeVM.Articles = _unitOfWork.ArticleRepository.GetArticlesbyFaculty(id).ToList();

            return View("Index", homeVM);
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
            if (user != null && user.Password == password)
            {
                if (user.Password == password)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("UserRole", user.Role);
                    if (user.Name != null && user.avtUrl != null)
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


    }
}
