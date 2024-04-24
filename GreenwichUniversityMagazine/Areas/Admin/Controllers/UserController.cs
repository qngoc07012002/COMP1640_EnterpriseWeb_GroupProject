using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Serivces.IServices;
using System.Text;
using System.Security.Cryptography;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        public const string Accept = "Accept";
        public const string Denied = "Denied";
        private readonly IEmailService _emailService;
        public UserController(IUnitOfWork db, IWebHostEnvironment webhost, IEmailService emailService)
        {
            _unitOfWork = db;
            _webhost = webhost;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            IQueryable<User> UserList = _unitOfWork.UserRepository.GetAllUser().OrderByDescending(t => t.Id);

            List<User> objUserList = UserList.ToList();
            return View(objUserList);
        }
        public IActionResult Create()
        {
            UserVM userVM = new UserVM()
            {
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),

            }; return View(userVM);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(UserVM userVM)
        {
            //if (ModelState.IsValid)
            {
                if (_unitOfWork.UserRepository.CheckEmail(userVM.User) == false)
                {
                    TempData["error"] = "Email Already Used!";
                    return View(userVM);
                } else
                {
                    TempData["success"] = "User created successfully";
                    Random random = new Random();
                    string password = GenerateRandomPassword(15);
                    var subject = "Your Login Information";
                    var message = $"<p><strong>Hello,</strong></p>" +
                                  $"<p>Here are your login details:</p>" +
                                  $"<p><strong>Email:</strong> {userVM.User.Email}</p>" +
                                  $"<p><strong>Password:</strong> {password}</p>" +
                                  $"<p><strong>Please keep this information secure.</strong></p>" +
                                  $"<p>Thank you!</p>";
                    await _emailService.SendEmailAsync(userVM.User.Email, subject, message);

                    userVM.User.Password = HashPassword(password);
                    userVM.User.avtUrl = "/img/avtImg/gw.jpg";
                    userVM.User.Status = true;
                    _unitOfWork.UserRepository.Add(userVM.User);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");
                }
               
               
            }
            //else
            //{
            //    userVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
            //        .Select(u => new SelectListItem
            //        {
            //            Text = u.Name,
            //            Value = u.Id.ToString()
            //        });

            //    return View(userVM);
            //}
        }

        public IActionResult Edit(int? id)
        {
            UserVM userVM = new UserVM()
            {
                MyUsers = _unitOfWork.UserRepository.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),

            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            userVM.User = _unitOfWork.UserRepository.Get(u => u.Id == id);
            return View(userVM);
        }
        [HttpPost]
        public IActionResult Edit(UserVM userVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webhost.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string userPath = Path.Combine(wwwRootPath, "img", "avtImg");

                    if (!string.IsNullOrEmpty(userVM.User.avtUrl) && (userVM.User.avtUrl != "/img/avtImg/gw.jpg"))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, userVM.User.avtUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(userPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    userVM.User.avtUrl = Url.Content("~/img/avtImg/" + fileName);
                }
                _unitOfWork.UserRepository.Update(userVM.User);
                TempData["success"] = "User updated successfully";
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                userVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                           Select(u => new SelectListItem
                           {
                               Text = u.Name,
                               Value = u.Id.ToString()
                           }); return View(userVM);
            }
        }
        public IActionResult Delete(int? id)
        {
            UserVM userVM = new UserVM()
            {
                MyUsers = _unitOfWork.UserRepository.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),

            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            userVM.User = _unitOfWork.UserRepository.Get(u => u.Id == id);
            return View(userVM);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userToDelete = _unitOfWork.UserRepository.Get(u => u.Id == id);

            if (userToDelete == null)
            {
                return NotFound();
            }

            string imagePathRelative = userToDelete.avtUrl;

            if (!string.IsNullOrEmpty(imagePathRelative))
            {
                string wwwRootPath = _webhost.WebRootPath;
                string imagePath = Path.Combine(wwwRootPath, imagePathRelative.TrimStart('/'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.UserRepository.Remove(userToDelete);
            TempData["success"] = "User delete successfully";
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }



        static string GenerateRandomPassword(int length)
        {
            // Chuỗi chứa tất cả các ký tự có thể được sử dụng để tạo mật khẩu
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789$@";

            // StringBuilder để tạo và quản lý mật khẩu
            StringBuilder password = new StringBuilder();

            // Sử dụng hàm Random để tạo các vị trí ngẫu nhiên trong chuỗi chars
            Random random = new Random();

            // Thêm ký tự ngẫu nhiên từ chuỗi chars vào mật khẩu cho đến khi đạt độ dài mong muốn
            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            return password.ToString();
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
