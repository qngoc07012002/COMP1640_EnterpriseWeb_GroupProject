using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using GreenwichUniversityMagazine.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            return RedirectToAction("Index", "Home", new { area = "student" });
        }
        public IActionResult EditProfile()
        {
            int userId = int.Parse(HttpContext.Session.GetString("UserId"));
            User user = _unitOfWork.UserRepository.Get(b => b.Id == userId);
         
            var faculties = _unitOfWork.FacultyRepository.GetAll();

            // Create a new instance of UserVM and assign values
            UserVM userVM = new UserVM
            {
                User = user,
                MyFaculties = new SelectList(faculties, "Id", "Name", user.FacultyId)
            };

            return View(userVM);
        }
        [HttpPost]
        public IActionResult EditProfile(IFormFile? file, UserVM userVM)
        {
            // Retrieve the User object from UserVM
            User user = userVM.User;

            // Check if the password is correct
           
                string wwwRoothPath = _webhost.WebRootPath;
                if (file != null)
                {
                    string? fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Delete Old Image
                    var oldImagePath = user.avtUrl != null ? Path.Combine(wwwRoothPath, user.avtUrl.TrimStart('/')) : null;

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                    using (var fileStream = new FileStream(Path.Combine(wwwRoothPath, "img/avtImg/", fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    user.avtUrl = @"/img/avtImg/" + fileName;
                }

              

                // Update User object
                _unitOfWork.UserRepository.Update(user);
                HttpContext.Session.SetString("avtUrl", user.avtUrl);
                _unitOfWork.Save();
                TempData["success"] = "Change Successfully";

                // Retrieve the updated list of faculties
                var faculties = _unitOfWork.FacultyRepository.GetAll();

                // Update the MyFaculties property in UserVM
                userVM.MyFaculties = new SelectList(faculties, "Id", "Name", user.FacultyId);
            

            // Return the view with the updated UserVM model
            return View(userVM);
        }

    }
}
