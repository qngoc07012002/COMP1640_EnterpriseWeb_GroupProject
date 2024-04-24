using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface ICoordinateRepository : IRepository<Article>
    {
        IEnumerable<Article> GetArticlesByMagazineIdAndStatus(int? magazineId, string status);
        List<Article> GetArticlesByTermAndMagazine(int termId, int magazineId, string status);

    }
}
