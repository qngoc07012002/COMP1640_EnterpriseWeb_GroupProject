using GreenwichUniversityMagazine.Authentication;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthentication()]
    public class FacultyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public FacultyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            List<Faculty> faculties = _unitOfWork.FacultyRepository.GetAll().ToList();

            return View(faculties);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]

        public IActionResult Create(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Add(faculty);
                _unitOfWork.Save();
                TempData["success"] = "faculty created succesfully";
                return RedirectToAction("Index");
            }

            return View();

        }

        public IActionResult Edit(int? idb)
        {
            if (idb == null || idb == 0)
            {
                return NotFound();
            }
            Faculty? faculty = _unitOfWork.FacultyRepository.Get(c => c.Id == idb);

            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }
        [HttpPost]
        public IActionResult Edit(Faculty faculty)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.FacultyRepository.Update(faculty);
                _unitOfWork.Save();
                TempData["success"] = "Faculty updated succesfully";
                return RedirectToAction("Index");
            }

            return View();
        }
        public IActionResult Delete(int? idb)
        {
            if (idb == null || idb == 0)
            {
                return NotFound();
            }
            Faculty? faculty = _unitOfWork.FacultyRepository.Get(c => c.Id == idb);
            if (faculty == null)
            {
                return NotFound();
            }
            return View(faculty);
        }
        [HttpPost]
        public IActionResult Delete(Faculty faculty)
        {
            _unitOfWork.FacultyRepository.Remove(faculty);
            _unitOfWork.Save();
            TempData["success"] = "Faculty deleted succesfully";
            return RedirectToAction("Index");
        }
    }
}
