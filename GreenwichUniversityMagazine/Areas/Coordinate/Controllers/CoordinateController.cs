using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Areas.Coordinate.Controllers
{
    [Area("Coordinate")]
    //[Authorize(Roles = "Coordinate")]
    public class CoordinateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CoordinateController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
        }
        //public ActionResult Index(int? id)
        //{
        //    // Lấy Faculty ID từ thông tin User
        //    int.TryParse(HttpContext.Session.GetString("UserId"), out int userId);
        //    var user = _unitOfWork.UserRepository.GetById(userId);
        //    int facultyId = user.FacultyId.Value;
        //    CoordinateVM model = new CoordinateVM();

        //    var magazines = _unitOfWork.MagazineRepository.GetAll()
        //                                .Where(u => u.FacultyId == facultyId)
        //                                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title })
        //                                .ToList();

        //    // Lấy danh sách các bài báo thuộc các tạp chí đó
        //    var articles = _unitOfWork.ArticleRepository.GetAll()
        //                                .Where(article => magazines.Any(magazine => article.MagazinedId == int.Parse(magazine.Value)))
        //                                .ToList();

        //    model.AvailableMagazines = magazines;
        //    model.ListArticle = articles;
        //    return View(model);
        //}
        public ActionResult Index(int? id)
        {
            DateTime currentDateTime = DateTime.Now;
            var termlist = _unitOfWork.TermRepository.GetAll().ToList();
            // Lấy Faculty ID từ thông tin User
            int.TryParse(HttpContext.Session.GetString("UserId"), out int userId);
            var user = _unitOfWork.UserRepository.GetById(userId);
            int facultyId = user.FacultyId.Value;
            CoordinateVM model = new CoordinateVM();

            var magazines = _unitOfWork.MagazineRepository.GetAll()
                                    .Where(u => u.FacultyId == facultyId &&
                                           u.Term.StartDate <= currentDateTime &&
                                           u.Term.EndDate >= currentDateTime)
                                    .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Title })
                                    .ToList();
            model.AvailableMagazines = magazines;

            if (id.HasValue)
            {
                foreach( var term in termlist)
                {
                    if(currentDateTime >= term.StartDate && currentDateTime <= term.EndDate)
                    {
                        var selectedMagazineId = id.Value;
                        var articles = _unitOfWork.ArticleRepository.GetAll()
                                                    .Where(article => article.MagazinedId == selectedMagazineId && article.Magazines.TermId == term.Id)
                                                    .ToList();
                        model.ListArticle = articles;
                    }
                }
            }
            else
            {
                List<Article> allArticles = new List<Article>();

                foreach (var term in termlist)
                {
                    if (currentDateTime >= term.StartDate && currentDateTime <= term.EndDate)
                    {
                        var articles = _unitOfWork.ArticleRepository.GetAll()
                                                    .Where(article => article.Magazines.TermId == term.Id)
                                                    .ToList();
                        allArticles.AddRange(articles);
                    }
                }
                model.ListArticle = allArticles;
            }
            return View(model);
        }



        public ActionResult ViewArticle(int? id)
        {
            List<Article> listArticle = _unitOfWork.ArticleRepository.GetAll()
                .Where(i => i.MagazinedId == id)
                .ToList();
            //List<User> ListUser = _unitOfWork.UserRepository.GetAll().ToList();
            List<Magazines> magazines = _unitOfWork.MagazineRepository.GetAll().ToList();
            CoordinateVM faculty = new CoordinateVM()
            {
                ListArticle = listArticle,
                ListMagazines = magazines
                    .ToList()
            };
            return View(faculty);
        }
        public IActionResult Detail(int? id)
        {
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int studentId);
            //List<Article> listArticles = _unitOfWork.ArticleRepository.GetAll().ToList();
            List<Resource> listResources = _unitOfWork.ResourceRepository.GetAll().Where(u => u.ArticleId == id).ToList();
            CoordinateVM cordinateVM = new CoordinateVM()
            {
                AvailableMagazines = _unitOfWork.MagazineRepository.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                }),
                MyResources = _unitOfWork.ResourceRepository.GetAll().Where(u => u.ArticleId == id).Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Path.ToString(),
                }),
                MyComments = _unitOfWork.CommentRepository.GetAll().Where(u => u.ArticleId == id && u.Type == "PRIVATE").ToList(),
                //ListArticle = listArticles,
                ListResource = listResources,
        };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            cordinateVM.articles = _unitOfWork.ArticleRepository.Get(u => u.ArticleId == id);
            return View(cordinateVM);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetNotification()
        {
            //get current UserID
            var userid = 1;
            //get all notification from DB
            //  List<Notification> notifications = _unitOfWork.NotificationRepository.GetAll().ToList();
            var notifications = new List<Notification>();
            notifications.Add(new Notification { id = 1, description = "Des1" });
            // Return partial view with the data
            return PartialView("_Notification", notifications);
        }
        public IActionResult changeStatus(int id)
        {
            var article = _unitOfWork.ArticleRepository.GetById(id);
            article.Status = !article.Status;
            _unitOfWork.ArticleRepository.Update(article);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
