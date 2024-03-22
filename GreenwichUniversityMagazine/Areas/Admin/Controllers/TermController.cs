using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
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
            Term term = new Term();
            return PartialView("Create",term);
        }

        [HttpPost]
        public IActionResult Create(Term obj, string Season, int Year)
        {
            if (string.IsNullOrEmpty(Season) || Year == 0)
            {
                TempData["error"] = "Season or Year cannot be null";
                return RedirectToAction("Index");
            }
            obj.Name = Season + " " + Year;
            var termList = _unitOfWork.TermRepository.GetAll().ToList();
            if (Season == "Summer")
            {
                obj.StartDate = new DateTime(Year, 6, 1); 
                obj.EndDate = new DateTime(Year, 8, 31); 
            }
            if (Season == "Winter")
            {
                obj.StartDate = new DateTime(Year, 12, 1); 
                obj.EndDate = new DateTime(Year + 1, 2, 28); 
            }
            if (Season == "Spring")
            {
                obj.StartDate = new DateTime(Year, 3, 1); 
                obj.EndDate = new DateTime(Year, 5, 31); 
            }
            if (Season == "Fall")
            {
                obj.StartDate = new DateTime(Year, 9, 1); 
                obj.EndDate = new DateTime(Year, 11, 30); 
            }
            bool termExists = false;
            foreach (var term in termList)
            {
                if (term.Name == obj.Name && term.IsDeleted == false)
                {
                    termExists = true;
                    break;
                }
            }
            if (termExists)
            {
                TempData["error"] = "Term already create";
            }
            else
            {
                _unitOfWork.TermRepository.Add(obj);
                TempData["success"] = "Term added successfully";
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        //public IActionResult Edit(int? id)
        //{
    //        if (id == null || id == 0)
    //        {
    //            return NotFound();
    //}
    //    Term? termFromDb = _unitOfWork.TermRepository.Get(u => u.Id == id);
    //    if (termFromDb == null)
    //    {
    //        return NotFound();
    //    }
    //    if(termFromDb.IsDeleted == true)
    //    {
    //        return RedirectToAction("Index");
    //    }
    //    return View(termFromDb);
    //}
    //[HttpPost]
    //public IActionResult Edit(Term obj)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        string[] parts = obj.Name.Split(' ');
    //        string season = parts[0];
    //        if (obj != null && obj.StartDate < obj.EndDate)
    //        {
    //            var magazinesToUpdate = _unitOfWork.MagazineRepository.GetAll().ToList();
    //            foreach (var magazine in magazinesToUpdate)
    //            {
    //                if (magazine.TermId == obj.Id && magazine.IsDeleted == false)
    //                {
    //                    if (magazine.StartDate < obj.StartDate)
    //                    {
    //                        magazine.StartDate = obj.StartDate;
    //                    }
    //                    if (magazine.EndDate > obj.EndDate)
    //                    {
    //                        magazine.EndDate = obj.EndDate;
    //                    }
    //                    if (magazine.StartDate > magazine.EndDate)
    //                    {
    //                        magazine.EndDate = magazine.StartDate;
    //                    }
    //                    if (magazine.StartDate > obj.EndDate)
    //                    {
    //                        magazine.StartDate = obj.StartDate;
    //                        magazine.EndDate = obj.EndDate;
    //                    }
    //                    _unitOfWork.MagazineRepository.Update(magazine);
    //                }
    //            }

    //            _unitOfWork.TermRepository.Update(obj);
    //            _unitOfWork.Save();

    //            TempData["success"] = "Term updated successfully";
    //            return RedirectToAction("Index");
    //        }
    //        else
    //        {
    //            TempData["popupScript"] = "alert('Start date must be less than End date.');";
    //        }
    //    }
    //    return View(obj);
    //}

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
            var magazineList = _unitOfWork.MagazineRepository.GetAll().ToList();
            if (termToDelete == null)
            {
                return NotFound();
            }
            termToDelete.IsDeleted = true;
            foreach(var magazine in  magazineList)
            {
                if(magazine.TermId == id)
                {
                    magazine.IsDeleted = true;
                    _unitOfWork.MagazineRepository.Update(magazine);
                }
            }
            _unitOfWork.TermRepository.Update(termToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Term delete successfully";
            return RedirectToAction("Index");
        }
    }
}
