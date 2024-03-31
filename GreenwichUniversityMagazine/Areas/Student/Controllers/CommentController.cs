using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("Student")]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public IActionResult UploadPrivate(int articleId, string commentInput)
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
                            Type = "PRIVATE",
                            Date = DateTime.Now
                        };

                        // Thêm comment mới vào cơ sở dữ liệu
                        _unitOfWork.CommentRepository.Add(newComment);
                        _unitOfWork.Save();

                        return Ok("Comment uploaded successfully.");
                    }
                    else
                    {
                        return Unauthorized("User is not authenticated."); // Trả về lỗi 401 nếu người dùng không xác thực
                    }
                }
                else
                {
                    return Unauthorized("User is not authenticated."); // Trả về lỗi 401 nếu người dùng không xác thực
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error uploading comment: " + ex.Message); // Trả về lỗi 400 nếu có lỗi xảy ra
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
                        return Unauthorized("User is not authenticated."); // Trả về lỗi 401 nếu người dùng không xác thực
                    }
                }
                else
                {
                    return Unauthorized("User is not authenticated."); // Trả về lỗi 401 nếu người dùng không xác thực
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
