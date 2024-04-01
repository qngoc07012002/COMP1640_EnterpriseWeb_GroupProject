using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {

        public Article GetById(int id);

        public IEnumerable<int> GetArticleCountsByTerm();
        public Task<IEnumerable<object>> GetMultipleLineChartData();
        public Task<IEnumerable<object>> GetGroupBarChart();
        public Task<IEnumerable<object>> GetStackBarChartContributions();

        IEnumerable<Article> Search(string searchString);

        List<Article> GetArticlesbyMagazine(int? id);
        List<Article> GetArticlesbyTerm(int? id);
        List<Article> GetArticlesbyFaculty(int? id);

    }
}
