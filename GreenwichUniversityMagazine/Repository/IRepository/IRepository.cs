using GreenwichUniversityMagazine.Models;
using System.Linq.Expressions;

namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperty = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperty = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Update(T entity);
    }
}