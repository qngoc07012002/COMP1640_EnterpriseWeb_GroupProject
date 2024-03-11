using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Cordinate.Controllers
{
    [Area("cordinate")]
    public class CordinateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CordinateController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

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
