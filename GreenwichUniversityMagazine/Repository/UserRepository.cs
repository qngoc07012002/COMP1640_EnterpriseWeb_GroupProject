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

        public bool CheckEmail(User user)
        {
            bool check = true;
            var query = _dbContext.Users.FirstOrDefault(m => m.Email == user.Email && m.Id != user.Id);
            if (query == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckPassword(int userId, string password)
        {
            var count = _dbContext.Users.Count(m => m.Id == userId && m.Password == password);
            if (count == 0)
            {
                return false;
            }
            else return true;
        }
        public User Login(string email, string password)
        {
            User? account = _dbContext.Users.FirstOrDefault(m => m.Email == email && m.Password == password);
            return account;

        }

        public void Register(User user)
        {
            _dbContext.Add(user);
            _dbContext.SaveChanges();
        }
    }
}
