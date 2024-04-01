using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class MagazineRepository : Repository<Magazines>, IMagazineRepository
    {
        private dbContext _dbContext;
        public MagazineRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }

        public List<Magazines> GetAllMagazine()
        {
            var query = _dbContext.Magazines.Where(c => c.Id != 0);
            return query.ToList();
        }
        /* public List<Article> GetAllMagazine()
         {
             var query = _dbContext.Articles.Where(c => c.MagazinedId != 0);
             return query.ToList();
         }*/
    }
}
