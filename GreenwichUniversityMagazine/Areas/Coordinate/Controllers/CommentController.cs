using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using GreenwichUniversityMagazine.Serivces.IServices;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Coordinate.Controllers
{
    [Area("Coordinate")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _webhost;
        private readonly string domain = "";
        public IActionResult Index()
        {
            return View();
        }

        public CommentController(IUnitOfWork db, IWebHostEnvironment webhost, IEmailService emailService)
        {
            _unitOfWork = db;
            _emailService = emailService;
            _webhost = webhost;
        }

        #region API CALLs

        [HttpPost]
        public IActionResult UploadPrivate(string CommentInput, int articleId)
        {
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int coordinateId);
            if (coordinateId > 0)
            {
                try
                {
                    Comment newComment = new();
                    newComment.UserId = coordinateId;
                    newComment.ArticleId = articleId;
                    newComment.Description = CommentInput;
                    newComment.Type = "PRIVATE";
                    newComment.Date = DateTime.Now;
                    newComment.IsNotification = true;
                    Article article = _unitOfWork.ArticleRepository.GetById(articleId);
                    User student = _unitOfWork.UserRepository.GetById(article.UserId);
                    var subject = "New Comment From Coordinate";
                    var message = $"The Coordinate just add new comment to article '{article.Title}'.\n Let check it";
                    _emailService.SendEmailAsync(student.Email.ToString(), subject, message);
                    _unitOfWork.CommentRepository.Add(newComment);
                    _unitOfWork.Save();
                    return Ok("Comment uploaded successfully.");
                }
                catch (Exception)
                {
                    return BadRequest("Invalid Request.");
                }
            }
            else
            {
                return BadRequest("NOT FOUND.");
            }
        }
        #endregion
    }
}
