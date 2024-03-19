using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("Student")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        public IActionResult Index()
        {
            return View();
        }

        public CommentController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }

        #region API CALLs

        [HttpPost]
        public IActionResult UploadPrivate(string CommentInput, int articleId)
        {
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int studentId);
            if (studentId > 0)
            {
                try
                {
                    Comment newComment = new();
                    newComment.ArticleId = articleId;
                    newComment.Description = CommentInput;
                    newComment.Type = "PRIVATE";
                    newComment.Date = DateTime.Now;
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