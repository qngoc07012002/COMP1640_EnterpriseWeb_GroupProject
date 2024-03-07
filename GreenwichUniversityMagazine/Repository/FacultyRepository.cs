using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        private dbContext _dbContext;
        public FacultyRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
    }
}
