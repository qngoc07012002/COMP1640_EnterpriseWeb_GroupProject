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
    }
}
