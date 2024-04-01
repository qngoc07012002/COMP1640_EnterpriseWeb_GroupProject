using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface ITermRepository : IRepository<Term>
    {
        Term GetById(int id);
        /*List<Term> GetAllTerm();*/
        List<Magazines> GetAllTerm();
    }
}
