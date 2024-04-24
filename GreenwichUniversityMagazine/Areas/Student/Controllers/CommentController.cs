using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using GreenwichUniversityMagazine.Serivces.IServices;
using Microsoft.AspNetCore.Mvc;
using System;

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
                    List<User> coordinates = _unitOfWork.UserRepository.GetAll().Where(u => u.FacultyId == student.FacultyId && u.Role.ToUpper() == "COORDINATE").ToList();
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
        [HttpPost]
        public IActionResult UploadPublic(int articleId, string commentInput)
        {
            try
            {
                // Kiểm tra xem session có chứa thông tin người dùng đã đăng nhập không
                var userId = HttpContext.Session.GetString("UserId");
                if (!string.IsNullOrEmpty(userId))
                {
                    // Chuyển đổi UserId từ chuỗi sang kiểu int
                    if (int.TryParse(userId, out int parsedUserId))
                    {
                        // Tạo một đối tượng Comment mới
                        Comment newComment = new Comment
                        {
                            UserId = parsedUserId,
                            ArticleId = articleId,
                            Description = commentInput,
                            Type = "PUBLIC",
                            Date = DateTime.Now,
                            IsNotification = true
                        };

                        // Thêm comment mới vào cơ sở dữ liệu
                        _unitOfWork.CommentRepository.Add(newComment);
                        _unitOfWork.Save();

                        return RedirectToAction("SelectArticle", "Article", new { id = articleId });
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else
                {
                    // Trả về lỗi 400 nếu có lỗi xảy ra
                    return BadRequest();

                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error uploading comment: " + ex.Message); // Trả về lỗi 400 nếu có lỗi xảy ra
            }
        }
        public IActionResult GetComments(int articleId)
        {
            // Lấy danh sách các comment từ cơ sở dữ liệu dựa vào articleId
            var comments = _unitOfWork.CommentRepository.GetAll().Where(c => c.ArticleId == articleId).ToList();

            // Trả về một PartialView chứa danh sách comment
            return PartialView("_CommentPartial", comments);
        }
    }
}
