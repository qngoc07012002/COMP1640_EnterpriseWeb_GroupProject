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
        //public ActionResult Index()
        //{
        //    int.TryParse(HttpContext.Session.GetString("UserId"), out int CoordinateID);
        //    CoordinateVM faculty = new CoordinateVM()
        //    {

        //        ListMagazines = _unitOfWork.MagazineRepository.GetAll().ToList(),
        //        AvailableFaculties = _unitOfWork.FacultyRepository.GetAll().
        //                    Select(u => new SelectListItem
        //                    {
        //                        Text = u.Name,
        //                        Value = u.Id.ToString()
        //                    }),

        //    };
        //    return View(faculty);
        //}
        public ActionResult Index()
        {
            // Lấy Faculty ID từ thông tin User
            int.TryParse(HttpContext.Session.GetString("UserId"), out int userId);
            var user = _unitOfWork.UserRepository.GetById(userId);
            int facultyId = user.FacultyId.Value;
            CoordinateVM model = new CoordinateVM();

            model.ListMagazines = _unitOfWork.MagazineRepository.GetAll().Where(u => u.FacultyId == facultyId).ToList();

            model.AvailableFaculties = _unitOfWork.FacultyRepository.GetAll()
                                        .Select(u => new SelectListItem
                                        {
                                            Text = u.Name,
                                            Value = u.Id.ToString()
                                        }).ToList();
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
            List<Article> listArticles = _unitOfWork.ArticleRepository.GetAll().ToList();
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
                ListArticle = listArticles,
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
    }
}
