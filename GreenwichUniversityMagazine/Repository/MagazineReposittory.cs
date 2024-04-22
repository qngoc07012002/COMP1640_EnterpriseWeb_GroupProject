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

        //public List<Magazines> GetAllMagazine()
        //{
        //    var query = _dbContext.Magazines.Where(c => c.Id != 0);
        //    return query.ToList();
        //}
        /* public List<Article> GetAllMagazine()
         {
             var query = _dbContext.Articles.Where(c => c.MagazinedId != 0);
             return query.ToList();
         }*/
        public async Task<int> CountNumberOfMagazine(int rangeSort)
        {
            var latestTermIds = await _dbContext.Terms
       .OrderByDescending(t => t.EndDate)
       .Select(t => t.Id)
       .Take(rangeSort)
       .ToListAsync();

            var query = from magazine in _dbContext.Magazines
                        where latestTermIds.Contains(magazine.TermId)
                        select magazine;

            var totalMagazines = await query.CountAsync();


            return totalMagazines;

        }

        IQueryable<Magazines> IMagazineRepository.GetAllMagazine()
        {
            return _dbContext.Magazines
                      .Include(m => m.Faculty)
                      .Include(m => m.Term)
                      .AsQueryable();
        }
    }
}
