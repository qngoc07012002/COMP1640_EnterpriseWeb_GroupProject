using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IMagazineRepository : IRepository<Magazines>
    {

        /*List<Magazines> GetAllMagazineByArticle(int? ArticleId);*/
        /* public List<Magazines> GetMagazinesByArticles(string searchString);*/
        IQueryable<Magazines> GetAllMagazine();
        Task<int> CountNumberOfMagazine(int rangeSort);

    }
}
