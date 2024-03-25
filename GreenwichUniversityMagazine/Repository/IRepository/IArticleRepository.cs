using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;
namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {
        Article GetById(int id);
    }
}
