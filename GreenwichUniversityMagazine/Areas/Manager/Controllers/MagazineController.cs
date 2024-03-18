using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MagazineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public MagazineController(IUnitOfWork db)
        {
            _unitOfWork = db;
        }
        public IActionResult Index()
        {
            List<Magazines> objCategoryList = _unitOfWork.MagazineRepository.GetAll().ToList();
            return View(objCategoryList);
        }
        public IActionResult Create()
        {
            MagazineVM magazineVM = new MagazineVM()
            {
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),
                MyTerms = _unitOfWork.TermRepository.GetAll().Where(b => b.IsDeleted == false).
                            Select(u => new SelectListItem
                            {
                                Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()}",
                                Value = u.Id.ToString()
                            }),
            }; return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Create(MagazineVM magazineVM)
        {
            if (ModelState.IsValid)
            {
                var term = _unitOfWork.TermRepository.GetById(magazineVM.Magazines.TermId);

                if (term != null && magazineVM.Magazines.StartDate >= term.StartDate && magazineVM.Magazines.EndDate <= term.EndDate && magazineVM.Magazines.StartDate < magazineVM.Magazines.EndDate)
                {
                    _unitOfWork.MagazineRepository.Add(magazineVM.Magazines);
                    _unitOfWork.Save();
                    TempData["success"] = "Magazine created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Magazines.EndDate", "Magazine's time must be within the Term's time Or Enddate must be greater than Startdate.");
                }
            }

            magazineVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            magazineVM.MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()}",
                    Value = u.Id.ToString()
                });

            return View(magazineVM);
        }

        public IActionResult Edit(int? id)
        {
            MagazineVM magazineVM = new MagazineVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),
                MyTerms = _unitOfWork.TermRepository.GetAll().Where(b => b.IsDeleted == false).
                            Select(u => new SelectListItem
                            {
                                Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()}",
                                Value = u.Id.ToString()
                            }),

            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            magazineVM.Magazines = _unitOfWork.MagazineRepository.Get(u => u.Id == id);
            if(magazineVM.Magazines.IsDeleted == true)
            {
                return RedirectToAction("Index");
            }
            return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Edit(MagazineVM magazineVM)
        {
            if (ModelState.IsValid)
            {
                var term = _unitOfWork.TermRepository.GetById(magazineVM.Magazines.TermId);

                if (term != null && magazineVM.Magazines.StartDate >= term.StartDate && magazineVM.Magazines.EndDate <= term.EndDate && magazineVM.Magazines.StartDate < magazineVM.Magazines.EndDate)
                {
                    _unitOfWork.MagazineRepository.Update(magazineVM.Magazines);
                    TempData["success"] = "Magazine updated successfully";
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Magazines.EndDate", "Magazine's time must be within the Term's time Or Enddate must be greater than Startdate.");
                }
            }
            magazineVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
  .Select(u => new SelectListItem
  {
      Text = u.Name,
      Value = u.Id.ToString()
  });
            magazineVM.MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()}",
                    Value = u.Id.ToString()
                });
            return View(magazineVM);
        }
        public IActionResult Delete(int? id)
        {
            MagazineVM magazineVM = new MagazineVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll().
              Select(u => new SelectListItem
              {
                  Text = u.Title,
                  Value = u.Id.ToString()
              }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                          Select(u => new SelectListItem
                          {
                              Text = u.Name,
                              Value = u.Id.ToString()
                          }),
                MyTerms = _unitOfWork.TermRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()}",
                                Value = u.Id.ToString()
                            }),
            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            magazineVM.Magazines = _unitOfWork.MagazineRepository.Get(u => u.Id == id);
            return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var magazineToDelete = _unitOfWork.MagazineRepository.Get(u => u.Id == id);

            if (magazineToDelete == null)
            {
                return NotFound();
            }
            magazineToDelete.IsDeleted = true;
            _unitOfWork.MagazineRepository.Update(magazineToDelete);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
