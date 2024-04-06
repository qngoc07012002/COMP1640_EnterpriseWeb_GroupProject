using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private dbContext _dbContext;
        public CommentRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
        public async Task<int> CountNumberOfComment(int rangeSort)
        {
            var latestTermIds = await _dbContext.Terms
       .OrderByDescending(t => t.EndDate)
       .Select(t => t.Id)
       .Take(rangeSort)
       .ToListAsync();

            var query = from article in _dbContext.Articles
                        join magazine in _dbContext.Magazines on article.MagazinedId equals magazine.Id
                        where latestTermIds.Contains(magazine.TermId)
                        join comment in _dbContext.Comments on article.ArticleId equals comment.ArticleId
                        select comment;

            var totalComments = await query.CountAsync();


            return totalComments;

        }
    }
}
