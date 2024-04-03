using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        private readonly dbContext _dbContext;
        public FacultyRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
        public int GetNumbersOfItems(int userId)
        {
            int count = _dbContext.Faculties.Count(c => c.Id == userId);
            return count;
        }
        public List<Faculty> GetFacultyByUser(int userId)
        {
            var query = _dbContext.Faculties.Where(c => c.Id == userId);
            string includeProperties = "Book";
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
            return query.ToList();
        }

        public void Update(Faculty faculty)
        {
            _dbContext.Update(faculty);
        }
        public List<Faculty> GetAllFaculty()
        {
            var query = _dbContext.Faculties.Where(c => c.Id != 0);
            return query.ToList();
        }

        /*public List<Magazines> GetAllFaculty()
        {
            var query = _dbContext.Magazines.Where(c => c.FacultyId != 0);
            return query.ToList();
        }*/

    }
}
