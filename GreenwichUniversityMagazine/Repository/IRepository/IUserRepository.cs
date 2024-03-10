using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        void Register(User user);

    }
}
