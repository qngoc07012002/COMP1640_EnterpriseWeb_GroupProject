using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private dbContext _dbContext;
        public ArticleRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
        public Article GetById(int id)
        {
            return _dbContext.Articles.FirstOrDefault(t => t.ArticleId == id);
        }
    }
}
