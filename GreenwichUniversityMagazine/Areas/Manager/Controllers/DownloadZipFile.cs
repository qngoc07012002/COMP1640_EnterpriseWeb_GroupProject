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
using GreenwichUniversityMagazine.Repository;

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
        public IActionResult ArticlesByMagazineAndTerm(int? selectedMagazineId, int? selectedTermId)
        {
            var magazines = _unitOfWork.MagazineRepository.GetAll().Where(b => b.StartDate <= DateTime.Now).ToList();
            var terms = _unitOfWork.TermRepository.GetAll().Where(b => b.StartDate <= DateTime.Now).ToList();
            

            // Tạo SelectList cho dropdown của Magazine và Term
            ViewBag.Magazines = new SelectList(magazines, "Id", "Title", selectedMagazineId);
            ViewBag.Terms = new SelectList(terms, "Id", "Name", selectedTermId);

            List<ArticleVM> articleVMs = new List<ArticleVM>();

            var articles = _unitOfWork.ArticleRepository.GetAll();

            // Người dùng đã chọn cả Magazine và Term
            if (selectedMagazineId != null && selectedTermId != null)
            {
                var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                                        .Where(m => m.TermId == selectedTermId)
                                        .Select(m => m.Id)
                                        .ToList();
             

                articles = articles.Where(a => a.MagazinedId == selectedMagazineId && magazinesInTerm.Contains(a.MagazinedId));
            }

            // Người dùng chỉ chọn Magazine hoặc chỉ chọn Term
            if (selectedMagazineId != null || selectedTermId != null)
            {
                if (selectedMagazineId != null)
                {
                    articles = articles.Where(a => a.MagazinedId == selectedMagazineId);
                }
                if (selectedTermId != null)
                {
                    var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                                            .Where(m => m.TermId == selectedTermId)
                                            .Select(m => m.Id)
                                            .ToList();

                    articles = articles.Where(a => magazinesInTerm.Contains(a.MagazinedId));
                }
               
            }
            articleVMs.AddRange(articles.Select(article => new ArticleVM { article = article }));

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
        public IActionResult DownloadFile(int articleId, string[] imageUrls, Article article)
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
                var articles = _unitOfWork.ArticleRepository.GetById(articleId);
                article = articles;
                    if (!string.IsNullOrEmpty(article.Body))
                    {
                        var pdfBytes = GeneratePdfFromString(article.Body);
                        var entryPathInArchive = $"article_{article.ArticleId}_body.pdf";
                        var entry = archive.CreateEntry(entryPathInArchive, CompressionLevel.Optimal);
                        using (var entryStream = entry.Open())
                        {
                            entryStream.Write(pdfBytes, 0, pdfBytes.Length);
                        }
                }
                

            }
            return PhysicalFile(zipFilePath, "application/zip", $"article_.zip");

        }

        public IActionResult DownloadAllArticlesByMagazineAndTerm(int? selectedMagazineId, int? selectedTermId)
        {
            List<Article> articles = new List<Article>();
            var nameZipFile = "";
            // Trường hợp người dùng chỉ chọn Term
            if (selectedTermId != null && selectedMagazineId == null)
            {
                var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                    .Where(m => m.TermId == selectedTermId)
                    .ToList();
                foreach (var magazine in magazinesInTerm)
                {
                    var articlesInMagazine = _unitOfWork.ArticleRepository.GetAll()
                        .Where(a => a.MagazinedId == magazine.Id && a.Status == true)
                        .ToList();

                    articles.AddRange(articlesInMagazine);
                }
                var terms = _unitOfWork.TermRepository.GetAll().Where(t => t.Id == selectedTermId);
                foreach (var term in terms)
                {
                    nameZipFile = term.Name;
                }
               
                
            }
            // Trường hợp người dùng chỉ chọn Magazine
            else if (selectedTermId == null && selectedMagazineId != null)
            {
                
                var articlesInMagazine = _unitOfWork.ArticleRepository.GetAll()
                    .Where(a => a.MagazinedId == selectedMagazineId && a.Status == true)
                    .ToList();
                articles.AddRange(articlesInMagazine);

                var magazine = _unitOfWork.MagazineRepository.GetAll().Where(m => m.Id == selectedMagazineId).ToList();
                foreach (var magazines in magazine)
                {
                    nameZipFile = magazines.Title;
                }



            }
            // Trường hợp người dùng không chọn gì cả
            else if (selectedTermId == null && selectedMagazineId == null)
            {
                var term = _unitOfWork.TermRepository.GetAll();
                foreach (var terms in term)
                {
                    var magazinesInTerm = _unitOfWork.MagazineRepository.GetAll()
                    .Where(m => m.TermId == terms.Id)
                    .ToList();
                    foreach (var magazine in magazinesInTerm)
                    {
                        var articlesInMagazine = _unitOfWork.ArticleRepository.GetAll()
                            .Where(a => a.MagazinedId == magazine.Id && a.Status == true)
                            .ToList();

                        articles.AddRange(articlesInMagazine);
                    }
                }
                nameZipFile = "All Article";
            }


            // Trường hợp người dùng đã chọn cả Term và Magazine
            else if (selectedTermId != null && selectedMagazineId != null)
            {
                var articlesInMagazine = _unitOfWork.ArticleRepository.GetAll()
                    .Where(a => a.MagazinedId == selectedMagazineId && a.Status == true)
                    .ToList();

                articles.AddRange(articlesInMagazine);
                var tempName1 = "";
                var tempName2 = "";

                var magazine = _unitOfWork.MagazineRepository.GetAll().Where(m => m.Id == selectedMagazineId).ToList();
                foreach (var magazines in magazine)
                {
                    tempName1 = magazines.Title;
                }
                var terms = _unitOfWork.TermRepository.GetAll().Where(t => t.Id == selectedTermId);
                foreach (var term in terms)
                {
                    tempName2 = term.Name;
                }

                nameZipFile = $"articles of {tempName1} in {tempName2}";


            }


            var zipFileName = $"{nameZipFile}.zip";
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
                        var entryPathInArchive = $"article_{article.ArticleId}/article_body.pdf";
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

