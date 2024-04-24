using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;
using GreenwichUniversityMagazine.Authentication;

namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("Manager")]
    [ManagerAuthentication()]
    public class MagazineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MagazineController(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = db;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            IQueryable<Magazines> objCategoryList = _unitOfWork.MagazineRepository.GetAllMagazine()
          .OrderByDescending(t => t.StartDate);

            List<Magazines> magazinesList = objCategoryList.ToList();

            return View(magazinesList);
        }
        public IActionResult Create()
        {
            DateTime currentDateTime = DateTime.Now;
            MagazineVM magazineVM = new MagazineVM()
            {
                MyFaculties = _unitOfWork.FacultyRepository.GetAll()
                              .Select(u => new SelectListItem
                              {
                                  Text = u.Name,
                                  Value = u.Id.ToString()
                              }),
                MyTerms = _unitOfWork.TermRepository.GetAll()
                          .Where(u => u.StartDate > currentDateTime)
                          .Select(u => new SelectListItem
                          {
                              Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()} / {u.Name}",
                              Value = u.Id.ToString(),
                          }),
            };
            return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Create(MagazineVM magazineVM)
        {
            if (ModelState.IsValid)
            {
                var term = _unitOfWork.TermRepository.GetById(magazineVM.Magazines.TermId);

                if (term != null && magazineVM.Magazines.StartDate >= term.StartDate && magazineVM.Magazines.EndDate <= term.EndDate && magazineVM.Magazines.StartDate < magazineVM.Magazines.EndDate)
                {
                    _unitOfWork.MagazineRepository.Add(magazineVM.Magazines);
                    _unitOfWork.Save();
                    TempData["success"] = "Magazine created successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Magazines.EndDate", "Magazine's time must be within the Term's time Or Enddate must be greater than Startdate.");
                }
            }

            magazineVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            magazineVM.MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()} / {u.Name}",
                    Value = u.Id.ToString(),
                });

            return View(magazineVM);
        }

        public IActionResult Edit(int? id)
        {
            MagazineVM magazineVM = new MagazineVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll().
                Select(u => new SelectListItem
                {
                    Text = u.Title,
                    Value = u.Id.ToString()
                }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                            Select(u => new SelectListItem
                            {
                                Text = u.Name,
                                Value = u.Id.ToString()
                            }),
                MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()} / {u.Name}",
                    Value = u.Id.ToString(),
                }),
            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            magazineVM.Magazines = _unitOfWork.MagazineRepository.Get(u => u.Id == id);
            return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Edit(MagazineVM magazineVM)
        {
            if (ModelState.IsValid)
            {
                var term = _unitOfWork.TermRepository.GetById(magazineVM.Magazines.TermId);

                if (term != null && magazineVM.Magazines.StartDate >= term.StartDate && magazineVM.Magazines.EndDate <= term.EndDate && magazineVM.Magazines.StartDate < magazineVM.Magazines.EndDate)
                {
                    _unitOfWork.MagazineRepository.Update(magazineVM.Magazines);
                    TempData["success"] = "Magazine updated successfully";
                    _unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Magazines.EndDate", "Magazine's time must be within the Term's time Or Enddate must be greater than Startdate.");
                }
            }
            magazineVM.MyFaculties = _unitOfWork.FacultyRepository.GetAll()
  .Select(u => new SelectListItem
  {
      Text = u.Name,
      Value = u.Id.ToString()
  });
            magazineVM.MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()} / {u.Name}",
                    Value = u.Id.ToString(),
                });
            return View(magazineVM);
        }
        public IActionResult Delete(int? id)
        {
            MagazineVM magazineVM = new MagazineVM()
            {
                MyMagazines = _unitOfWork.MagazineRepository.GetAll().
              Select(u => new SelectListItem
              {
                  Text = u.Title,
                  Value = u.Id.ToString()
              }),
                MyFaculties = _unitOfWork.FacultyRepository.GetAll().
                          Select(u => new SelectListItem
                          {
                              Text = u.Name,
                              Value = u.Id.ToString()
                          }),
                MyTerms = _unitOfWork.TermRepository.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = $"{u.StartDate.ToString()} - {u.EndDate.ToString()} / {u.Name}",
                    Value = u.Id.ToString(),
                }),
            };
            if (id == null || id == 0)
            {
                return NotFound();
            }
            magazineVM.Magazines = _unitOfWork.MagazineRepository.Get(u => u.Id == id);
            return View(magazineVM);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var magazineToDelete = _unitOfWork.MagazineRepository.Get(u => u.Id == id);
            var articlelist = _unitOfWork.ArticleRepository.GetAll().ToList();

            if (magazineToDelete == null)
            {
                return NotFound();
            }
            foreach (var article in articlelist)
            {
                if (article.MagazinedId == id)
                {
                    _unitOfWork.ArticleRepository.Remove(article);
                }
            }
            _unitOfWork.MagazineRepository.Remove(magazineToDelete);
            _unitOfWork.Save();
            TempData["success"] = "Magazine delete successfully";
            return RedirectToAction("Index");
        }
        public IActionResult ArticlesByMagazine(int? selectedMagazineId)
        {
            var magazines = _unitOfWork.MagazineRepository.GetAll().ToList().Where(b=>b.StartDate <= DateTime.Now);
            ViewBag.Magazines = new SelectList(magazines, "Id", "Title", selectedMagazineId);

            List<ArticleVM> articleVMs = new List<ArticleVM>();

            if (selectedMagazineId != null)
            {
                var articles = _unitOfWork.ArticleRepository.GetAll()
                                .Where(a => a.MagazinedId == selectedMagazineId)  
                                .ToList();

                foreach (var article in articles)
                {
                    var articleVM = new ArticleVM
                    {
                        article = article,
                    };
                    articleVMs.Add(articleVM);
                }

                return View(articleVMs);
            }

            return View(articleVMs);
        }
        public IActionResult ArticlesByTerm(int? selectedTermId)
        {
            var terms = _unitOfWork.TermRepository.GetAll().ToList().Where(b => b.StartDate <= DateTime.Now);
            ViewBag.Terms = new SelectList(terms, "Id", "Name", selectedTermId);

            List<ArticleVM> articleVMs = new List<ArticleVM>();

            if (selectedTermId != null)
            {
                var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                                        .Where(m => m.TermId == selectedTermId)
                                        .ToList();

                foreach (var magazine in magazinesInTerm)
                {
                    var articles = _unitOfWork.ArticleRepository.GetAll()
                                    .Where(a => a.MagazinedId == magazine.Id)
                                    .ToList();

                    foreach (var article in articles)
                    {
                        var articleVM = new ArticleVM
                        {
                            article = article,
                        };
                        articleVMs.Add(articleVM);
                    }
                }

                return View(articleVMs);
            }

            return View(articleVMs);
        }

        public byte[] GeneratePdfFromString(string text)
        {
            string plainText = Regex.Replace(text, "<.*?>", string.Empty);
            plainText = System.Net.WebUtility.HtmlDecode(plainText);

            using (MemoryStream ms = new MemoryStream())
            {
                using (iTextSharp.text.Document document = new iTextSharp.text.Document())
                {
                    PdfWriter.GetInstance(document, ms);

                    document.Open();
                    document.Add(new Paragraph(plainText));
                    document.Close();
                }

                return ms.ToArray();
            }
        }
        public IActionResult DownloadFile(int articleId, string[] imageUrls, string bodyText)
        {
            var articleFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Resource", "Article", articleId.ToString());
            var zipFilePath = Path.Combine(_hostEnvironment.WebRootPath, "fileDownloaded", $"article_{articleId}.zip");

            if (System.IO.File.Exists(zipFilePath))
            {
                System.IO.File.Delete(zipFilePath);
            }

            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                if (Directory.Exists(articleFolderPath))
                {
                    foreach (var filePath in Directory.GetFiles(articleFolderPath))
                    {
                        var entryName = Path.GetFileName(filePath);
                        archive.CreateEntryFromFile(filePath, entryName);
                    }
                }

                foreach (var imageUrl in imageUrls)
                {
                    var imageName = Path.GetFileName(imageUrl);
                    var imagePhysicalPath = Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePhysicalPath))
                    {
                        archive.CreateEntryFromFile(imagePhysicalPath, imageName);
                    }
                    else
                    {

                    }
                }

                if (!string.IsNullOrEmpty(bodyText))
                {
                    var pdfBytes = GeneratePdfFromString(bodyText);
                    archive.CreateEntry("article_body.pdf").Open().Write(pdfBytes, 0, pdfBytes.Length);
                }
            }

            return PhysicalFile(zipFilePath, "application/zip", $"article_{articleId}.zip");
        }

        public IActionResult DownloadAllArticlesByMagazine(int selectedMagazineId)
        {
            var articles = _unitOfWork.ArticleRepository.GetAll()
                .Where(a => a.MagazinedId == selectedMagazineId && a.Status == true)
                .ToList();

            var zipFileName = $"all_articles_of_magazine{selectedMagazineId}.zip";
            var zipFilePath = Path.Combine(_hostEnvironment.WebRootPath, "fileDownloaded", zipFileName);

            if (System.IO.File.Exists(zipFilePath))
            {
                System.IO.File.Delete(zipFilePath);
            }

            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var article in articles)
                {
                    var articleFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Resource", "Article", article.ArticleId.ToString());

                    if (Directory.Exists(articleFolderPath))
                    {
                        var folderName = $"article_{article.ArticleId}";
                        var folderPathInArchive = folderName + "/";
                        archive.CreateEntry(folderPathInArchive); 

                        foreach (var filePath in Directory.GetFiles(articleFolderPath))
                        {
                            var entryName = Path.GetFileName(filePath);
                            var entryPathInArchive = folderPathInArchive + entryName;
                            archive.CreateEntryFromFile(filePath, entryPathInArchive);
                        }
                    }

                    var imageUrl = article.imgUrl; 
                    var imageName = Path.GetFileName(imageUrl);
                    var imagePhysicalPath = Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePhysicalPath))
                    {
                        using (var fileStream = new FileStream(imagePhysicalPath, FileMode.Open))
                        {
                            var entryPathInArchive = $"article_{article.ArticleId}/{imageName}";
                            var entry = archive.CreateEntry(entryPathInArchive, CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(article.Body))
                    {
                        var pdfBytes = GeneratePdfFromString(article.Body);
                        var entryPathInArchive = $"article_{article.ArticleId}/article_{article.ArticleId}_body.pdf";
                        var entry = archive.CreateEntry(entryPathInArchive, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(pdfBytes, 0, pdfBytes.Length);
                        }
                    }
                }
            }

            return PhysicalFile(zipFilePath, "application/zip", zipFileName);
        }
        public IActionResult DownloadAllArticlesByTerm(int selectedTermId)
        {
            var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                .Where(m => m.TermId == selectedTermId)
                .ToList();

            List<Article> articles = new List<Article>();

            foreach (var magazine in magazinesInTerm)
            {
                var articlesInMagazine = _unitOfWork.ArticleRepository.GetAll()
                    .Where(a => a.MagazinedId == magazine.Id && a.Status == true)
                    .ToList();

                articles.AddRange(articlesInMagazine);
            }

            var zipFileName = $"all_articles_of_term_{selectedTermId}.zip";
            var zipFilePath = Path.Combine(_hostEnvironment.WebRootPath, "fileDownloaded", zipFileName);

            if (System.IO.File.Exists(zipFilePath))
            {
                System.IO.File.Delete(zipFilePath);
            }

            using (var archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var article in articles)
                {
                    var articleFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Resource", "Article", article.ArticleId.ToString());

                    if (Directory.Exists(articleFolderPath))
                    {
                        var folderName = $"article_{article.ArticleId}";
                        var folderPathInArchive = folderName + "/";
                        archive.CreateEntry(folderPathInArchive); 

                        foreach (var filePath in Directory.GetFiles(articleFolderPath))
                        {
                            var entryName = Path.GetFileName(filePath);
                            var entryPathInArchive = folderPathInArchive + entryName;
                            archive.CreateEntryFromFile(filePath, entryPathInArchive);
                        }
                    }

                    var imageUrl = article.imgUrl; 
                    var imageName = Path.GetFileName(imageUrl);
                    var imagePhysicalPath = Path.Combine(_hostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePhysicalPath))
                    {
                        using (var fileStream = new FileStream(imagePhysicalPath, FileMode.Open))
                        {
                            var entryPathInArchive = $"article_{article.ArticleId}/{imageName}";
                            var entry = archive.CreateEntry(entryPathInArchive, CompressionLevel.Optimal);
                            using (var entryStream = entry.Open())
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(article.Body))
                    {
                        var pdfBytes = GeneratePdfFromString(article.Body);
                        var entryPathInArchive = $"article_{article.ArticleId}/article_{article.ArticleId}_body.pdf";
                        var entry = archive.CreateEntry(entryPathInArchive, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(pdfBytes, 0, pdfBytes.Length);
                        }
                    }
                }
            }

            return PhysicalFile(zipFilePath, "application/zip", zipFileName);
        }



    }
}

