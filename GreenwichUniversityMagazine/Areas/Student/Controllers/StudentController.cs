using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Repository.IRepository;
using System.Text;
using System.Security.Cryptography;
using GreenwichUniversityMagazine.Authentication;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    [CommonAuthentication()]
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
            if (ModelState.IsValid)
            {
                try
                {

                    string wwwRootPath = _webhost.WebRootPath;

                    if (file != null)
                    {

                        string basePath = Path.Combine("img", "avtImg");

                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string newPath = Path.Combine(basePath, fileName);


                        string oldImagePath = Path.Combine(wwwRootPath, userVM.User.avtUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath) && (userVM.User.avtUrl != "/img/avtImg/gw.jpg"))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(wwwRootPath, newPath), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        userVM.User.avtUrl = Url.Content("~/img/avtImg/" + fileName);
                    }
                    _unitOfWork.UserRepository.Update(userVM.User);
                    _unitOfWork.Save();
                    HttpContext.Session.SetString("avtUrl", userVM.User.avtUrl);
                    HttpContext.Session.SetString("UserName", userVM.User.Name);
                    TempData["success"] = "Change Successfully";
                    return RedirectToAction("EditProfile", "Student", new { area = "Student" });

                }


                catch (Exception ex)
                {
                    // Xử lý ngoại lệ nếu có
                    TempData["error"] = "An error occurred while updating profile: " + ex.Message;
                    var faculties = _unitOfWork.FacultyRepository.GetAll();
                    userVM.MyFaculties = new SelectList(faculties, "Id", "Name", userVM.User.FacultyId);
                    return View(userVM);
                }
            }
            else
            {
                var faculties = _unitOfWork.FacultyRepository.GetAll();
                userVM.MyFaculties = new SelectList(faculties, "Id", "Name", userVM.User.FacultyId);
                return View(userVM);
            }
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
                if (!_unitOfWork.UserRepository.CheckPassword(userId, HashPassword(currentPassword)))
                {
                    TempData["error"] = "Invalid Password";
                }
                else
                {
                    User user = _unitOfWork.UserRepository.Get(b => b.Id == userId);
                    user.Password = HashPassword(newPassword);
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.Save();
                    TempData["success"] = "Update Password Successfully";
                }
            }
            return View();
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
    }
}
