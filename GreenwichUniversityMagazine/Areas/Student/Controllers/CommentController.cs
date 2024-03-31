using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Serivces.IServices;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using GreenwichUniversityMagazine.Serivces;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("Student")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        private readonly IEmailService _emailService;
        private readonly string domain = "";

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
            int.TryParse(UserIdGet, out int studentId);
            if (studentId > 0)
            {
                try
                {
                    Comment newComment = new();
                    newComment.UserId = studentId;
                    newComment.ArticleId = articleId;
                    newComment.Description = CommentInput;
                    newComment.Type = "PRIVATE";
                    newComment.Date = DateTime.Now;
                    newComment.IsNotification = true;
                    //Sending email 
                    Article article = _unitOfWork.ArticleRepository.GetById(articleId);
                    User student = _unitOfWork.UserRepository.GetById(studentId);  
                    List<User> coordinates = _unitOfWork.UserRepository.GetAll().Where(u=> u.FacultyId == student.FacultyId && u.Role.ToUpper() == "COORDINATE").ToList();
                    var subject = "New Comment From Student";
                    var message = $"The Student just add new comment to article '{article.Title}'.\n Let check it: {domain}/Coordinate/Coordinate/{articleId}";
                    foreach (var coor in coordinates)
                    {
                        _emailService.SendEmailAsync(coor.Email.ToString(), subject, message);
                    }

                    //Done
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