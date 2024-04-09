using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("student")]
    public class ViewController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;

        public ViewController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }

        public IActionResult Index(string? searchString, int? magazineid, int? termid, int? facultyid, int? articlesid)
        {
            ViewVM model = new ViewVM();
            model.Terms = _unitOfWork.TermRepository.GetAll().ToList();
            model.Facultys = _unitOfWork.FacultyRepository.GetAll().ToList();
            model.Magazines = _unitOfWork.MagazineRepository.GetAll().ToList();

           
            if (!string.IsNullOrEmpty(searchString))
            {
                model.Articles = _unitOfWork.ArticleRepository.Search(searchString).ToList();
            }
            else if (magazineid.HasValue)
            {
                model.Articles = _unitOfWork.ArticleRepository.GetArticlesbyMagazine(magazineid.Value).ToList();
                ViewBag.MagazineName = _unitOfWork.MagazineRepository.Get(u => u.Id == magazineid.Value).Title;
            }
            else if (termid.HasValue)
            {
                model.Articles = _unitOfWork.ArticleRepository.GetArticlesbyTerm(termid.Value).ToList();
                ViewBag.TermName = _unitOfWork.TermRepository.Get(u => u.Id == termid.Value).Name;
            }
            else if (facultyid.HasValue)
            {
                model.Articles = _unitOfWork.ArticleRepository.GetArticlesbyFaculty(facultyid.Value).ToList();
                ViewBag.FacultyName = _unitOfWork.FacultyRepository.Get(u => u.Id == facultyid.Value).Name;
            }
            else
            {
                model.Articles = _unitOfWork.ArticleRepository.GetAll().Where(a => a.Status == true).OrderByDescending(a => a.ArticleId).ToList();
            }

            return View(model);
        }





    }
}
