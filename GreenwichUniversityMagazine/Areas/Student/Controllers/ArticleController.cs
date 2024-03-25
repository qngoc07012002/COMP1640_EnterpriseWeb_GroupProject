    using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenwichUniversityMagazine.Areas.Student.Controllers
{
    [Area("Student")]
    public class ArticleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webhost;

        public ArticleController(IUnitOfWork db, IWebHostEnvironment webhost)
        {
            _unitOfWork = db;
            _webhost = webhost;
        }

        public IActionResult Index()
        {
            int.TryParse(HttpContext.Session.GetString("UserId"), out int StudentId);
            List <Article> articles = _unitOfWork.ArticleRepository.GetAll(includeProperty: "Magazines").Where(u=> u.UserId == StudentId).ToList();
            return View(articles);
        }
        public IActionResult Create()
        {
            ArticleVM articleVM = new ArticleVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.Id.ToString()
                    })
            };
            return View(articleVM);
        }

        public IActionResult Update(int id)
        {
            try
            {
                ArticleVM articleVM = new ArticleVM()
                {
                    MyMagazines = _unitOfWork.MagazineRepository.GetAll().
                    Select(u => new SelectListItem
                    {
                        Text = u.Title,
                        Value = u.Id.ToString()
                    }),
                    MyComments = _unitOfWork.CommentRepository.GetAll().Where(u => u.ArticleId == id && u.Type == "PRIVATE").ToList(),
                    article = new Article()
                    

                };
                var UserIdGet = HttpContext.Session.GetString("UserId");
                int.TryParse(UserIdGet, out int studentId);
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

        #region API CALLs
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
                if (articleVM.article.ArticleId == 0)
                {
                    var UserIdGet = HttpContext.Session.GetString("UserId");
                    int.TryParse(UserIdGet, out int studentId);
                    articleVM.article.UserId = studentId;
                    articleVM.article.SubmitDate = DateTime.Today;
                    articleVM.article.ModifyDate = DateTime.Today;
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

                articleVM.MyMagazines = _unitOfWork.MagazineRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Title,
                                Value = u.Id.ToString()
                            });
                return View(articleVM);
            }

        }


        [HttpPost]
        public IActionResult Update(ArticleVM articleVM, IFormFile? HeadImg, List<IFormFile> files, string? filesDelete)
        {

            if (ModelState.IsValid)
            {
                ArticleVM articleVM2 = new();
                articleVM2.article = _unitOfWork.ArticleRepository.Get(u => u.ArticleId == articleVM.article.ArticleId);
                articleVM.article.imgUrl = articleVM2.article.imgUrl;
                string wwwRootPath = _webhost.WebRootPath;
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
                articleVM.article.ModifyDate = DateTime.Today;
                articleVM.article.Status = false;

                //Delete Old Files
                if (filesDelete !=null)
                {
                    int[] IdsToDelete = filesDelete.Split(',').Select(int.Parse).ToArray();
                    var oldImagePath = Path.Combine(wwwRootPath, articleVM.article.imgUrl.TrimStart('/'));
                    foreach (int i in IdsToDelete)
                    {
                        Resource resource = _unitOfWork.ResourceRepository.Get(u=> u.Id == i);
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
        #endregion

    }
}
