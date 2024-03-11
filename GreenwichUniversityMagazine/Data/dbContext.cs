using GreenwichUniversityMagazine.Models;
using Microsoft.EntityFrameworkCore;
namespace GreenwichUniversityMagazine.Data
{
    public class dbContext : DbContext
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Magazines> Magazines { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
