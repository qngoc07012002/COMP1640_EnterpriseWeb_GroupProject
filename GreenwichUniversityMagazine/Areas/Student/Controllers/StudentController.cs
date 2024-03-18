using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Repository.IRepository;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]

    public class StudentController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        public StudentController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }
        public IActionResult Index()
        {
            return View();
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
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword, string newPassword, string confirmNewPassword)
        {
            if (newPassword != confirmNewPassword)
            {
                TempData["error"] = "Password Doesn't Match";
            }
            else
            {
                int userId = int.Parse(HttpContext.Session.GetString("UserId"));
                if (!_unitOfWork.UserRepository.CheckPassword(userId, currentPassword))
                {
                    TempData["error"] = "Invalid Password";
                }
                else
                {
                    User user = _unitOfWork.UserRepository.Get(b => b.Id == userId);
                    user.Password = newPassword;
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();
                    TempData["success"] = "Update Password Successfully";
                }
            }
            return View();
        }

    }
}

