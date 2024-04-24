using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;

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

        public ActionResult Index(int? id, string status = "all")
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

            List<Magazines> magazineList = _unitOfWork.MagazineRepository.GetAll()
                                           .Where(u => u.FacultyId == facultyId &&
                                                  u.Term.StartDate <= currentDateTime &&
                                                  u.Term.EndDate >= currentDateTime)
                                           .ToList();
            if (id.HasValue || id.HasValue.ToString() == "all")
            {
                foreach (var term in termlist)
                {
                    if (currentDateTime >= term.StartDate && currentDateTime <= term.EndDate)
                    {
                        var selectedMagazineId = id.Value;
                        var articles = _unitOfWork.CoordinateRepository.GetArticlesByMagazineIdAndStatus(id, status);
                        model.ListArticle = articles.OrderByDescending(article => article.ArticleId).ToList();
                        break;
                    }
                }
            }
                else if (id == null)
                {
                    List<Article> allArticles = new List<Article>();

                    foreach (var term in termlist)
                    {
                        if (currentDateTime >= term.StartDate && currentDateTime <= term.EndDate)                                                                                                                                           
                        {
                            foreach (var magazine in magazineList)
                            {
                            var articles = _unitOfWork.CoordinateRepository.GetArticlesByTermAndMagazine(term.Id, magazine.Id, status);
                            allArticles.AddRange(articles);
                            }
                        }
                    }
                    model.ListArticle = allArticles.OrderByDescending(article => article.ArticleId).ToList();
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
        //public IActionResult GetPdf(int articleId, string fileName)
        //{
        //    var docxPath = $"~/Resource/Article/{articleId}/{fileName}";

        //    // Kiểm tra xem tệp có tồn tại không
        //    if (!System.IO.File.Exists(docxPath))
        //    {
        //        return NotFound();
        //    }

        //    // Đường dẫn để lưu trữ tệp PDF mới
        //    var pdfPath = $"~/Resource/Article/{articleId}/{Path.GetFileNameWithoutExtension(fileName)}.pdf";

        //    // Kiểm tra xem tệp PDF đã tồn tại chưa, nếu chưa thì tạo mới
        //    if (!System.IO.File.Exists(pdfPath))
        //    {
        //        // Chuyển đổi từ docx sang pdf
        //        using (DocX doc = DocX.Load(docxPath))
        //        {
        //            doc.SaveAs(Path.ChangeExtension(pdfPath, ".pdf"));
        //        }
        //    }

        //    // Chuyển hướng đến tệp PDF đã chuyển đổi
        //    return Redirect(pdfPath);
        //}


        //[HttpPost]
        //public IActionResult ConvertToPdf(int articleId, string fileName)
        //{

        //    // Kiểm tra xem fileName có phải là tệp docx không
        //    if (!fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        //    {
        //        return BadRequest("Invalid file format");
        //    }

        //    // Xác định đường dẫn của tệp docx
        //    var docxFilePath = Path.Combine(_fileStoragePath, "Article", articleId.ToString(), fileName);

        //    // Kiểm tra xem tệp docx có tồn tại không
        //    if (!System.IO.File.Exists(docxFilePath))
        //    {
        //        return NotFound("File not found");
        //    }

        //    try
        //    {
        //        // Đường dẫn lưu trữ tệp PDF
        //        var pdfFilePath = Path.Combine(_fileStoragePath, "Article", articleId.ToString(), Path.ChangeExtension(fileName, ".pdf"));

        //        // Load tệp docx bằng Aspose.Words và chuyển đổi nó thành PDF
        //        Aspose.Words.Document doc = new Aspose.Words.Document(docxFilePath);
        //        doc.Save(pdfFilePath, Aspose.Words.SaveFormat.Pdf);

        //        // Trả về đường dẫn của tệp PDF đã chuyển đổi
        //        return Ok(new { pdfPath = pdfFilePath });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"An error occurred while converting the file: {ex.Message}");
        //    }
        //}

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetNotification()
        {
            //get current UserID
            //get all notification from DB
            //  List<Notification> notifications = _unitOfWork.NotificationRepository.GetAll().ToList();
            var notifications = new List<Comment>();
            List<User> users = _unitOfWork.UserRepository.GetAll().ToList();
            List<Article> article = _unitOfWork.ArticleRepository.GetAll().ToList();
            var UserIdGet = HttpContext.Session.GetString("UserId");
            int.TryParse(UserIdGet, out int userIdCurrent);

            User user = _unitOfWork.UserRepository.GetById(userIdCurrent);
            Faculty faculty = _unitOfWork.FacultyRepository.Get(u => u.Id == user.FacultyId);
            List<Comment> comments;
            if (user.Role.ToLower() == "student")
            {
                comments = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.Article.UserId == userIdCurrent && u.UserId != userIdCurrent).ToList();
            }
            else
            {
                comments = _unitOfWork.CommentRepository.GetAll().Where(u => u.IsNotification == true && u.UserId != userIdCurrent && u.User.Role.ToLower() != "coordinate" && u.Type.ToLower() == "private" && u.Article.User.FacultyId == user.FacultyId).ToList();
            }
            // Return partial view with the data
            comments.Reverse();
            return PartialView("_Notification", comments);
        }

        public IActionResult changeStatus(int id)
        {
            var article = _unitOfWork.ArticleRepository.GetById(id);
            if(article.Status == false)
            {
                article.Status = true;
            }
            _unitOfWork.ArticleRepository.Update(article);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
