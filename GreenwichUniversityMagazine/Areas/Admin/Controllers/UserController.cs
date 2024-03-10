using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GreenwichUniversityMagazine.Models.ViewModels;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    [Area("admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        public const string Accept = "Accept";
        public const string Denied = "Denied";
        public UserController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }

        public IActionResult Index()
        {
            List<User> objUserList = _unitOfWork.UserRepository.GetAll().ToList();
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
        public IActionResult Create(UserVM userVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webhost.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string bookPath = Path.Combine(wwwRootPath, "img", "avtImg");
                using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                userVM.User.avtUrl = Url.Content("~/img/avtImg/" + fileName);
                userVM.User.Role = "Student";
                userVM.User.Status = false;

                _unitOfWork.UserRepository.Add(userVM.User);
                _unitOfWork.Save();

                TempData["success"] = "User created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                userVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });

                return View(userVM);
            }
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

                    if (!string.IsNullOrEmpty(userVM.User.avtUrl))
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

            string wwwRootPath = _webhost.WebRootPath;
            string imagePath = Path.Combine(wwwRootPath, imagePathRelative.TrimStart('/'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            _unitOfWork.UserRepository.Remove(userToDelete);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }


    }
}
