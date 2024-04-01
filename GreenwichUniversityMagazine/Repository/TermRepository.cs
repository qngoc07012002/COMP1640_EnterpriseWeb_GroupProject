using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;

namespace GreenwichUniversityMagazine.Repository
{
    public class TermRepository : Repository<Term>, ITermRepository
    {
        private readonly dbContext _dbContext;

        public TermRepository(dbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Term GetById(int id)
        {
            return _dbContext.Terms.FirstOrDefault(t => t.Id == id);
        }

        /* public List<Term> GetAllTerm()
         {
             var query = _dbContext.Terms.Where(c => c.Id != 0);
             return query.ToList();
         }
 */
        public List<Magazines> GetAllTerm()
        {
            var query = _dbContext.Magazines.Where(c => c.TermId != 0);
            return query.ToList();
        }

    }
}
