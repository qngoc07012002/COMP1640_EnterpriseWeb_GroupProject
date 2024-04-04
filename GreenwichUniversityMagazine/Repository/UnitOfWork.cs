using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Repository.IRepository;

namespace GreenwichUniversityMagazine.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private dbContext _dbContext;

        public IUserRepository UserRepository { get; private set; }

        public ICommentRepository CommentRepository { get; private set; }

        public IArticleRepository ArticleRepository { get; private set; }

        public IMagazineRepository MagazineRepository { get; private set; }

        public ITermRepository TermRepository { get; private set; }

        public IFacultyRepository FacultyRepository { get; private set; }

        public IResourceRepository ResourceRepository { get; private set; }

        public UnitOfWork(dbContext dbContext)
        {
            _dbContext = dbContext;
            UserRepository = new UserRepository(_dbContext);
            ResourceRepository = new ResourceRepository(_dbContext);
            CommentRepository = new CommentRepository(_dbContext);
            ArticleRepository = new ArticleRepository(_dbContext);
            MagazineRepository = new MagazineRepository(_dbContext);
            FacultyRepository = new FacultyRepository(_dbContext);
            ResourceRepository = new ResourceRepository(_dbContext);
            TermRepository = new TermRepository(_dbContext);
        }
        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}