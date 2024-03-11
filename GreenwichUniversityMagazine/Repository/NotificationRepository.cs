using Microsoft.EntityFrameworkCore;
using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
namespace GreenwichUniversityMagazine.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private dbContext _dbContext;
        public NotificationRepository(dbContext dbContext): base(dbContext) {
            _dbContext = dbContext;
        }
    }
}
