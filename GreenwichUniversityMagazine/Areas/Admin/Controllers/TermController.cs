using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace GreenwichUniversityMagazine.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TermController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public TermController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }
        public IActionResult Index()
        {
            List<Term> objTermList = _unitOfWork.TermRepository.GetAll().ToList();
            return View(objTermList);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Term obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null && obj.StartDate < obj.EndDate)
                {
                    _unitOfWork.TermRepository.Add(obj);
                    TempData["success"] = "Term added successfully";
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["popupScript"] = "alert('Start date must be less than End date.');";
                }
            }
            return View(obj);
        }
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Term? termFromDb = _unitOfWork.TermRepository.Get(u => u.Id == id);
            if (termFromDb == null)
            {
                return NotFound();
            }
            if(termFromDb.IsDeleted == true)
            {
                return RedirectToAction("Index");
            }
            return View(termFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Term obj)
        {
            if (ModelState.IsValid)
            {
                if (obj != null && obj.StartDate < obj.EndDate)
                {
                    var magazinesToUpdate = _unitOfWork.MagazineRepository.GetAll().ToList();

                    foreach (var magazine in magazinesToUpdate)
                    {
                        if (magazine.TermId == obj.Id && magazine.IsDeleted == false)
                        {
                            if (magazine.StartDate < obj.StartDate)
                            {
                                magazine.StartDate = obj.StartDate;
                            }
                            if (magazine.EndDate > obj.EndDate)
                            {
                                magazine.EndDate = obj.EndDate;
                            }
                            if (magazine.StartDate > magazine.EndDate)
                            {
                                magazine.EndDate = magazine.StartDate;
                            }
                            if (magazine.StartDate > obj.EndDate)
                            {
                                magazine.StartDate = obj.StartDate;
                                magazine.EndDate = obj.EndDate;
                            }
                            _unitOfWork.MagazineRepository.Update(magazine);
                        }
                    }

                    _unitOfWork.TermRepository.Update(obj);
                    _unitOfWork.Save();

                    TempData["success"] = "Term updated successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["popupScript"] = "alert('Start date must be less than End date.');";
                }
            }
            return View(obj);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Term? termFromDb = _unitOfWork.TermRepository.Get(u => u.Id == id);

            if (termFromDb == null)
            {
                return NotFound();
            }
            return View(termFromDb);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var termToDelete = _unitOfWork.TermRepository.Get(u => u.Id == id);

            if (termToDelete == null)
            {
                return NotFound();
            }
            termToDelete.IsDeleted = true;
            _unitOfWork.TermRepository.Update(termToDelete);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
