using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Index(int? id)
        {

            ViewVM model = new ViewVM();
            var magazines = _unitOfWork.MagazineRepository.GetAll()
                                    .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title })
                                    .ToList();
            model.MyMagazines = magazines;

            var terms = _unitOfWork.TermRepository.GetAll()
                            .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = $"{t.Name} ({t.StartDate.ToShortDateString()} - {t.EndDate.ToShortDateString()})" })
                            .ToList();
            model.MyTerms = terms;

            if (id.HasValue)
            {
                var selectedMagazineId = id.Value;
                var articles = _unitOfWork.ArticleRepository.GetAll()
                                            .Where(article => article.MagazinedId == selectedMagazineId)
                                            .ToList();
                model.ListArticle = articles;
            }
            else
            {
                
                var articles = _unitOfWork.ArticleRepository.GetAll().ToList();
                model.ListArticle = articles;
            }
            return View(model);
           
        }



       
    }
}
