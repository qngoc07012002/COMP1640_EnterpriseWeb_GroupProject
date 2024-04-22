using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GreenwichUniversityMagazine.Repository
{
    public class CoordinateRepository : Repository<Article>, ICoordinateRepository
    {
        private readonly dbContext _dbContext;
        public CoordinateRepository(dbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<Article> GetArticlesByMagazineIdAndStatus(int? magazineId, string status)
        {
            return _dbContext.Articles
                .Where(article => article.MagazinedId == magazineId &&
                                  (status == "all" || (status == "pending" && !article.Status) ||
                                   (status == "approved" && article.Status)))
                .OrderByDescending(article => article.ArticleId)
                .ToList();
        }
        public List<Article> GetArticlesByTermAndMagazine(int termId, int magazineId, string status)
        {
            return _dbContext.Articles
                .Where(article => article.Magazines.TermId == termId && article.MagazinedId == magazineId &&
                                  (status == "all" || (status == "pending" && !article.Status) ||
                                   (status == "approved" && article.Status)))
                .OrderByDescending(article => article.ArticleId)
                .ToList();
        }

    }
}

