using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using GreenwichUniversityMagazine.Repository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenwichUniversityMagazine.Data;

namespace GreenwichUniversityMagazine.Tests
{
    [TestFixture]
    public class CommentRepositoryTest
    {
        private dbContext _dbContext;
        private ICommentRepository _commentRepository;

        [SetUp]
        public void Setup()
        {
            // Create a mock of the database context
            var options = new DbContextOptionsBuilder<dbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new dbContext(options);

            // Add sample data to the in-memory database
            _dbContext.Terms.AddRange(
                new Term { Id = 1, Name = "Spring 2024", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-06-30") },
                new Term { Id = 2, Name = "Fall 2024", StartDate = DateTime.Parse("2024-07-01"), EndDate = DateTime.Parse("2024-12-31") }
            );

            _dbContext.Magazines.AddRange(
                new Magazines { Id = 1, Title = "Magazine 1", TermId = 1, Description = "Description 1" },
                new Magazines { Id = 2, Title = "Magazine 2", TermId = 2, Description = "Description 2" }
            );

            _dbContext.Articles.AddRange(
                new Article { ArticleId = 1, UserId = 1, MagazinedId = 1, Title = "Article 1", Body = "Body 1", SubTitle = "Subtitle 1", SubmitDate = DateTime.Now, ModifyDate = DateTime.Now, Status = true },
                new Article { ArticleId = 2, UserId = 2, MagazinedId = 2, Title = "Article 2", Body = "Body 2", SubTitle = "Subtitle 2", SubmitDate = DateTime.Now, ModifyDate = DateTime.Now, Status = true }
            );

            _dbContext.Comments.AddRange(
                new Comment { Id = 1, UserId = 1, ArticleId = 1, Description = "Comment 1", Date = DateTime.Now },
                new Comment { Id = 2, UserId = 2, ArticleId = 2, Description = "Comment 2", Date = DateTime.Now }
            );

            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _commentRepository = new CommentRepository(_dbContext);
        }



        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }


    }

}
