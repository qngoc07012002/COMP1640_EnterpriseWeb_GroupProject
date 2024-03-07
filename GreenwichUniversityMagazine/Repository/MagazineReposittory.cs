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
    }
}
