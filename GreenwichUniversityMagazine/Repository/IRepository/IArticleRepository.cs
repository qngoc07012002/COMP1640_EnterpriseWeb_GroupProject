using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {
        IEnumerable<int> GetArticleCountsByTerm();
        Task<IEnumerable<object>> GetMultipleLineChartData();
        Task<IEnumerable<object>> GetGroupBarChart();
        Task<IEnumerable<object>> GetStackBarChartContributions();

    }
}
