using GreenwichUniversityMagazine.Models;
using Microsoft.AspNetCore.Mvc;

namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IArticleRepository : IRepository<Article>
    {

        public Article GetById(int id);
        IEnumerable<int> GetArticleCountsByTerm(int rangeSort);
        Task<IEnumerable<object>> GetMultipleLineChartData(int rangeSort);
        Task<IEnumerable<object>> GetGroupBarChart(int rangeSort);
        Task<IEnumerable<object>> GetStackBarChartContributions(int rangeSort);
        Task<object> GetDoughnutChart(int rangeSort);
        Task<int> CountNumberOfUnapproved(int rangeSort);

        IEnumerable<Article> Search(string searchString);

        List<Article> GetArticlesbyMagazine(int? id);
        List<Article> GetArticlesbyTerm(int? id);
        List<Article> GetArticlesbyFaculty(int? id);

    }
}
