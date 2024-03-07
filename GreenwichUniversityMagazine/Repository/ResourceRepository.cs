using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class ResourceRepository : Repository<Resource>, IResourceRepository
    {
        private dbContext _dbContext;
        public ResourceRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
    }
}
