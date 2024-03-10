using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private dbContext _dbContext;
        public UserRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
        public void Register(User user)
        {
            _dbContext.Add(user);
            _dbContext.SaveChanges();
        }
    }
}
