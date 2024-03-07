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
    }
}
