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
    public class ArticleRepositoryTest
    {
        private dbContext _dbContext;
        private IArticleRepository _articleRepository;

        [SetUp]
        public void Setup()
        {
            // Create a mock of the database context
            var options = new DbContextOptionsBuilder<dbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new dbContext(options);

            // Add sample data to the in-memory database
            var faculty = new Faculty { Name = "Sample Faculty" };
            var term = new Term { Name = "Spring 2024", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-12-31") };
            var user = new User { Email = "test@example.com", Password = "password", Name = "Test User", Faculty = faculty, Role = "Admin", Status = true };
            var magazine = new Magazines { Title = "Sample Magazine", Description = "Test Description", Faculty = faculty, Term = term, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) };

            _dbContext.Faculties.Add(faculty);
            _dbContext.Terms.Add(term);
            _dbContext.Users.Add(user);
            _dbContext.Magazines.Add(magazine);
            _dbContext.SaveChanges();

            _dbContext.Articles.Add(new Article
            {
                UserId = user.Id,
                MagazinedId = magazine.Id,
                Title = "Test Article",
                SubTitle = "Test Subtitle",
                imgUrl = "https://example.com/image.jpg",
                Body = "Sample Body",
                SubmitDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = true
            });
            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _articleRepository = new ArticleRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetById_ValidId_ReturnsArticle()
        {
            // Arrange
            int id = 1;

            // Act
            var article = _articleRepository.GetById(id);

            // Assert
            ClassicAssert.IsNotNull(article);
            ClassicAssert.AreEqual("Test Article", article.Title);
        }

        [Test]
        public void GetById_InvalidId_ReturnsNull()
        {
            // Arrange
            int id = 999;

            // Act
            var article = _articleRepository.GetById(id);

            // Assert
            ClassicAssert.IsNull(article);
        }

        [Test]
        public void Search_ReturnsMatchingArticles()
        {
            // Arrange
            string searchString = "Test";

            // Act
            var articles = _articleRepository.Search(searchString);

            // Assert
            ClassicAssert.IsNotEmpty(articles);
            ClassicAssert.IsTrue(articles.All(a => a.Title.Contains(searchString) || a.Body.Contains(searchString)));
        }

        [Test]
        public void GetArticlesbyMagazine_ReturnsArticlesForMagazine()
        {
            // Arrange
            int magazineId = 1;

            // Act
            var articles = _articleRepository.GetArticlesbyMagazine(magazineId);

            // Assert
            ClassicAssert.IsNotEmpty(articles);
            ClassicAssert.IsTrue(articles.All(a => a.MagazinedId == magazineId));
        }

        [Test]
        public void GetArticlesbyTerm_ReturnsArticlesForTerm()
        {
            // Arrange
            int termId = 1;

            // Act
            var articles = _articleRepository.GetArticlesbyTerm(termId);

            // Assert
            ClassicAssert.IsNotEmpty(articles); // Adjust this assertion based on your implementation
            ClassicAssert.IsTrue(articles.All(a => a.Magazines.TermId == termId)); // Check if all articles belong to the expected term
        }


        [Test]
        public void GetArticlesbyFaculty_ReturnsArticlesForFaculty()
        {
            // Arrange
            int facultyId = 1;

            // Act
            var articles = _articleRepository.GetArticlesbyFaculty(facultyId);

            // Assert
            ClassicAssert.IsNotEmpty(articles);
            ClassicAssert.IsTrue(articles.All(a => a.Magazines.FacultyId == facultyId)); // Check if all articles belong to the expected faculty
        }

        [Test]
        public void GetAll_ReturnsAllArticles()
        {
            // Act
            var articles = _articleRepository.GetAll().ToList();

            // Assert
            ClassicAssert.IsNotNull(articles);
            ClassicAssert.AreEqual(1, articles.Count); // Adjust the count based on the sample data
        }

        [Test]
        public void Add_NewArticle_AddsArticleToDatabase()
        {
            // Arrange
            var newArticle = new Article
            {
                UserId = 1,
                MagazinedId = 1,
                Title = "New Test Article",
                SubTitle = "New Test Subtitle",
                imgUrl = "https://example.com/new_image.jpg",
                Body = "New Sample Body",
                SubmitDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Status = true
            };

            // Act
            _articleRepository.Add(newArticle);
            _dbContext.SaveChanges(); // Save changes to ensure the article is added to the database

            // Assert
            var addedArticle = _dbContext.Articles.FirstOrDefault(a => a.Title == newArticle.Title);
            ClassicAssert.IsNotNull(addedArticle);
        }

        [Test]
        public void Remove_ExistingArticle_RemovesArticleFromDatabase()
        {
            // Arrange
            var articleIdToRemove = 1;

            // Act
            var articleToRemove = _dbContext.Articles.FirstOrDefault(a => a.ArticleId == articleIdToRemove);
            _articleRepository.Remove(articleToRemove);
            _dbContext.SaveChanges(); // Save changes to ensure the article is removed from the database

            // Assert
            var removedArticle = _dbContext.Articles.FirstOrDefault(a => a.ArticleId == articleIdToRemove);
            ClassicAssert.IsNull(removedArticle);
        }

        [Test]
        public void RemoveRange_MultipleArticles_RemovesArticlesFromDatabase()
        {
            // Act
            var articlesToRemove = _dbContext.Articles.ToList();
            _articleRepository.RemoveRange(articlesToRemove);
            _dbContext.SaveChanges(); // Save changes to ensure articles are removed from the database

            // Assert
            var remainingArticlesCount = _dbContext.Articles.Count();
            ClassicAssert.AreEqual(0, remainingArticlesCount);
        }

        [Test]
        public void Update_ExistingArticle_UpdatesArticleInDatabase()
        {
            // Arrange
            var existingArticleId = 1;
            var updatedTitle = "Updated Test Article";

            // Act
            var existingArticle = _dbContext.Articles.FirstOrDefault(a => a.ArticleId == existingArticleId);
            existingArticle.Title = updatedTitle;
            _articleRepository.Update(existingArticle);
            _dbContext.SaveChanges(); // Save changes to ensure the article is updated in the database

            // Assert
            var updatedArticle = _dbContext.Articles.FirstOrDefault(a => a.ArticleId == existingArticleId);
            ClassicAssert.IsNotNull(updatedArticle);
            ClassicAssert.AreEqual(updatedTitle, updatedArticle.Title);
        }

    }


}
