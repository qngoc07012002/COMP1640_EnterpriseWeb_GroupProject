using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IFacultyRepository : IRepository<Faculty>
    {

        public int GetNumbersOfItems(int userId);
        public List<Faculty> GetFacultyByUser(int userId);
        void Update(Faculty faculty);


    }
}
