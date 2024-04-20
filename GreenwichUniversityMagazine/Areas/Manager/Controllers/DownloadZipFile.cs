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

namespace GreenwichUniversityMagazine.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class DownloadZipFile : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public DownloadZipFile(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = db;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ArticlesByMagazine(int? selectedMagazineId)
        {
            var magazines = _unitOfWork.MagazineRepository.GetAll().ToList().Where(b => b.StartDate <= DateTime.Now);
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
