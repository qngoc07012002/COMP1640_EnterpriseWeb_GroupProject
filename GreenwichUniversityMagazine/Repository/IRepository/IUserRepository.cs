using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {

        User Login(string email, string password);
        void Register(User user);
        bool CheckPassword(int userId, string password);
        bool CheckEmail(User user);
        User GetById(int id);
        int GetNumberOfStudents();

    }
}
