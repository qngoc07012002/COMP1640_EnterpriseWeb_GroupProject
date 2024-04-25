    using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Globalization;
using System.Xml.Linq;
using GreenwichUniversityMagazine.Serivces.IServices;
using GreenwichUniversityMagazine.Authentication;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("Student")]
  
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;
        private readonly IEmailService _emailService;


        public ArticleController(IUnitOfWork db, IWebHostEnvironment webhost, IEmailService emailService)
        {
            _unitOfWork = db;
            _webhost = webhost;
            _emailService = emailService;
        }
        [StudentAuthentication()]
        public IActionResult Index()
        {
            return View();
        }
        [StudentAuthentication()]
        public IActionResult Create()
        {
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int studentId);
            User user = _unitOfWork.UserRepository.GetById(studentId);
            ArticleVM articleVM = new ArticleVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Term").Where(u =>
                    u.EndDate >= DateTime.Now &&
                    u.StartDate <= DateTime.Now &&
                    u.FacultyId == user.FacultyId &&
                    u.Term.EndDate > u.EndDate &&
                    u.StartDate >= u.Term.StartDate).Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.Id.ToString()
                    })
            };
            return View(articleVM);
        }
        [StudentAuthentication()]
        public IActionResult Update(int id, string? status)
        {
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int studentId);
            User user = _unitOfWork.UserRepository.GetById(studentId);
            try
            {
                ArticleVM articleVM = new ArticleVM()
                {
                    MyMagazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Term").Where(u =>
                    u.EndDate >= DateTime.Now &&
                    u.StartDate <= DateTime.Now &&
                    u.FacultyId == user.FacultyId &&
                    u.Term.EndDate > u.EndDate &&
                    u.StartDate >= u.Term.StartDate)
                    .Select(u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.Id.ToString()
                    }),
                    MyComments = _unitOfWork.CommentRepository.GetAll().Where(u => u.ArticleId == id && u.Type == "PRIVATE").ToList(),
                    article = new Article(),
                    status = status

                };
                //Update
                articleVM.article = _unitOfWork.ArticleRepository.Get(article => article.ArticleId == id);
                if (studentId == articleVM.article.UserId)
                {
                    articleVM.MyResources = _unitOfWork.ResourceRepository.GetAll().Where(u => u.ArticleId == id).Select(u => new SelectListItem
                    {
                        Value = u.Id.ToString(),
                        Text = u.Path.ToString(),
                    });

                    return View(articleVM);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (NullReferenceException)
            {
                return RedirectToAction("Error", "Home");
            }

        }
        public IActionResult SelectArticle(int id)
        {
            Article article = _unitOfWork.ArticleRepository.Get(
                includeProperty: "Magazines,User",
                filter: a => a.ArticleId == id
            );
            if (article == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Comment> comments = _unitOfWork.CommentRepository.GetAll()
            .Where(c => c.ArticleId == id && c.Type.ToUpper() == "PUBLIC")
            .ToList();

            List<User> commentUsers = new List<User>();
            foreach (var comment in comments)
            {
                User user = _unitOfWork.UserRepository.Get(u => u.Id == comment.UserId);
                commentUsers.Add(user);
            }
            
            ArticleVM articleVM = new ArticleVM
            {
                article = article,
                User = article.User,
                Magazines = article.Magazines,
                FormattedModifyDate = article.ModifyDate?.ToString("dd/MM/yyyy"),
                MyComments = comments,
                CommentUsers = commentUsers,
                Terms = _unitOfWork.TermRepository.GetAll().ToList(),
                Facultys = _unitOfWork.FacultyRepository.GetAll().ToList(),
                Magazine = _unitOfWork.MagazineRepository.GetAll().ToList(),
                Articles = _unitOfWork.ArticleRepository.GetAll().Where(a => a.Status == true).ToList()
        };

            
            return View(articleVM);
        }

        #region API CALLs
        [StudentAuthentication()]
        [HttpPost]
        public IActionResult Create(ArticleVM articleVM, IFormFile? HeadImg, List<IFormFile> files)
        {
            if (ModelState.IsValid && HeadImg != null)
            {
                string wwwRootPath = _webhost.WebRootPath;
                if (HeadImg != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HeadImg.FileName);
                    string bookPath = Path.Combine(wwwRootPath, "img/articleImg");
                    using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
                    {
                        HeadImg.CopyTo(fileStream);
                    }
                    articleVM.article.imgUrl = "/img/articleImg/" + fileName;
                }
                else
                {
                    ArticleVM articleVM2 = new();
                    articleVM2.article = _unitOfWork.ArticleRepository.Get(u => u.ArticleId == articleVM.article.ArticleId);
                }
                //Create Done
                if (articleVM.article.ArticleId == 0)
                {
                    var UserIdGet = HttpContext.Session.GetString("UserId");
                    int.TryParse(UserIdGet, out int studentId);
                    articleVM.article.UserId = studentId;
                    articleVM.article.SubmitDate = DateTime.Now;
                    articleVM.article.ModifyDate = DateTime.Now;
                    User student = _unitOfWork.UserRepository.GetById(studentId);

                    //Sending email 
                    List<User> coordinates = _unitOfWork.UserRepository.GetAll().Where(u => u.FacultyId == student.FacultyId && u.Role.ToUpper() == "COORDINATE").ToList();
                    var subject = "New Article From Student";
                    var message = $"The Student just add new article '{articleVM.article.Title}'.\n Let check it.";
                    foreach (var coor in coordinates)
                    {
                        _emailService.SendEmailAsync(coor.Email.ToString(), subject, message);
                    }
                    _unitOfWork.ArticleRepository.Add(articleVM.article);
                    _unitOfWork.Save();
                    TempData["success"] = "Article created succesfully, Please wait managerment confirm.";
                }
                if (files.Count > 0)
                {
                    int articleId = articleVM.article.ArticleId;

                    foreach (var file in files)
                    {

                        string basePath = Path.Combine(wwwRootPath, "Resource", "Article", articleId.ToString());
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(basePath, fileName);

                        using (FileStream stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        Resource resource = new Resource
                        {
                            ArticleId = articleId,
                            Path = $"/Resource/Article/{articleId.ToString()}/{fileName}",
                            Type = Path.GetExtension(fileName)
                        };
                        _unitOfWork.ResourceRepository.Add(resource);
                        _unitOfWork.Save();
                    }

                }


                return RedirectToAction("Index");
            }
            else
            {
                var UserIdGet = HttpContext.Session.GetString("UserId");
                int.TryParse(UserIdGet, out int studentId);
                User user = _unitOfWork.UserRepository.GetById(studentId);
                articleVM.MyMagazines = _unitOfWork.MagazineRepository.GetAll(includeProperty: "Term")
                    .Where(u => 
                    u.EndDate >= DateTime.Now &&
                    u.StartDate <= DateTime.Now &&
                    u.FacultyId == user.FacultyId && 
                    u.Term.EndDate > u.EndDate &&
                    u.StartDate >= u.Term.StartDate).
                            Select(u => new SelectListItem
                            {
                                Text = u.Title,
                                Value = u.Id.ToString()
                            });
                return View(articleVM);
            }

        }

        [StudentAuthentication()]
        [HttpPost]
        public IActionResult Update(ArticleVM articleVM, IFormFile? HeadImg, List<IFormFile> files, string? filesDelete, string? body2)
        {
            string wwwRootPath = _webhost.WebRootPath;
            ArticleVM articleVM2 = new();
            articleVM2.article = _unitOfWork.ArticleRepository.Get(u => u.ArticleId == articleVM.article.ArticleId);
            Magazines magazines = _unitOfWork.MagazineRepository.Get(u => u.Id == articleVM2.article.MagazinedId);
            Term term = _unitOfWork.TermRepository.Get(u => u.Id == magazines.TermId);
            if (magazines.EndDate < DateTime.Now)
            {
                if (body2 != null)
                {
                    articleVM2.article.Body = articleVM2.article.Body + body2;
                    _unitOfWork.ArticleRepository.Update(articleVM2.article);
                    _unitOfWork.Save();

                }
                if (files.Count > 0)
                {
                    int articleId = articleVM.article.ArticleId;
                    foreach (var file in files)
                    {
                        string basePath = Path.Combine(wwwRootPath, "Resource", "Article", articleId.ToString());
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(basePath, fileName);

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            System.IO.File.Delete(filePath);
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        else
                        {
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        if (System.IO.File.Exists(filePath))
                        {
                            Resource resource = new Resource
                            {
                                ArticleId = articleId,
                                Path = $"/Resource/Article/{articleId.ToString()}/{fileName}",
                                Type = Path.GetExtension(fileName)
                            };
                            _unitOfWork.ResourceRepository.Add(resource);
                            _unitOfWork.Save();
                        }
                    }

                }

                TempData["success"] = "Article update file succesfully!";
                return RedirectToAction("Index");

            }
            if (ModelState.IsValid)
            {
                articleVM.article.imgUrl = articleVM2.article.imgUrl;
                //Check file Img
                if (HeadImg != null)
                {
                    string basePath = Path.Combine("img", "articleImg");
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(HeadImg.FileName);
                    string newPath = Path.Combine(basePath, fileName);

                    var oldImagePath = Path.Combine(wwwRootPath, articleVM.article.imgUrl.TrimStart('/'));
                    //Delete Old Img
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine("Not Found this file !!");
                        }

                    }
                    using (var fileStream = new FileStream(Path.Combine(wwwRootPath, newPath), FileMode.Create))
                    {
                        HeadImg.CopyTo(fileStream);
                    }
                    articleVM.article.imgUrl = Url.Content("~/img/articleImg/" + fileName);
                }
                articleVM.article.UserId = articleVM2.article.UserId;
                articleVM.article.ModifyDate = DateTime.Now;
                articleVM.article.Status = false;

                //Delete Old Files
                if (filesDelete != null)
                {
                    int[] IdsToDelete = filesDelete.Split(',').Select(int.Parse).ToArray();
                    var oldImagePath = Path.Combine(wwwRootPath, articleVM.article.imgUrl.TrimStart('/'));
                    foreach (int i in IdsToDelete)
                    {
                        Resource resource = _unitOfWork.ResourceRepository.Get(u => u.Id == i);
                        var oldResource = Path.Combine(wwwRootPath, resource.Path.TrimStart('/'));
                        _unitOfWork.ResourceRepository.Remove(resource);
                        System.IO.File.Delete(oldResource);
                        _unitOfWork.Save();
                    }
                }

                //Update files
                if (files.Count > 0)
                {
                    int articleId = articleVM.article.ArticleId;

                    foreach (var file in files)
                    {
                        string basePath = Path.Combine(wwwRootPath, "Resource", "Article", articleId.ToString());
                        if (!Directory.Exists(basePath))
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        string fileName = Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(basePath, fileName);

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            System.IO.File.Delete(filePath);
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        else
                        {
                            using (FileStream stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                        }
                        if (System.IO.File.Exists(filePath))
                        {
                            Resource resource = new Resource
                            {
                                ArticleId = articleId,
                                Path = $"/Resource/Article/{articleId.ToString()}/{fileName}",
                                Type = Path.GetExtension(fileName)
                            };
                            _unitOfWork.ResourceRepository.Add(resource);
                            _unitOfWork.Save();
                        }
                    }

                }

                _unitOfWork.ArticleRepository.Update(articleVM.article);
                TempData["success"] = "Article updated succesfully, Please wait management confirm.";
                _unitOfWork.Save();
                //Done
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Update");
            }
        }
        [StudentAuthentication()]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var article = _unitOfWork.ArticleRepository.Get(u => u.ArticleId == id);
            if (article == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string wwwRootPath = _webhost.WebRootPath;
            string basePath = Path.Combine(wwwRootPath, "Resource", "Article", id.ToString());
            try
            {
                System.IO.File.Delete(article.imgUrl);
                if (Directory.Exists(basePath))
                {
                    Directory.Delete(basePath, true);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine("An error occurred while deleting the directory: " + ex.Message);
            }
            _unitOfWork.ArticleRepository.Remove(article);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        [StudentAuthentication()]
        [HttpGet]
        public IActionResult GetByStatus(string status, int page = 1, int pageSize = 6)
        {

            int.TryParse(HttpContext.Session.GetString("UserId"), out int StudentId);
            IEnumerable<Term> allTerms = _unitOfWork.TermRepository.GetAll();
            IEnumerable<Article> query = _unitOfWork.ArticleRepository.GetAll(includeProperty: "Magazines").Where(u => u.UserId == StudentId);
            
            if (status.ToLower() == "pending")
            {
                query = query.Where(u => u.Status == false);
            }
            else if (status.ToLower() == "success")
            {
                query = query.Where(u => u.Status == true);
            }
            int allArticleCount = query.Count();
            List<Article> articles = query.ToList();
            int skipCount = (page - 1) * pageSize;
            articles = articles.Skip(skipCount).Take(pageSize).ToList();

            return Json(new { allArticleCount, articles });

        }
        #endregion

    }
}
