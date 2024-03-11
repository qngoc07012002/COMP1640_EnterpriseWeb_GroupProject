namespace GreenwichUniversityMagazine.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICommentRepository CommentRepository { get; }
        IArticleRepository ArticleRepository { get; }
        IMagazineRepository MagazineRepository { get; }
        ITermRepository TermRepository { get; }
        IFacultyRepository FacultyRepository { get; }
        IResourceRepository ResourceRepository { get; }
        INotificationRepository NotificationRepository { get; }
        void Save();
    }
}
