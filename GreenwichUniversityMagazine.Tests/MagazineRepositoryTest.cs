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
    public class MagazineRepositoryTest
    {
        private dbContext _dbContext;
        private IMagazineRepository _magazineRepository;

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
            var term1 = new Term { Name = "Spring 2024", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-06-30") };
            var term2 = new Term { Name = "Fall 2024", StartDate = DateTime.Parse("2024-07-01"), EndDate = DateTime.Parse("2024-12-31") };

            _dbContext.Faculties.Add(faculty);
            _dbContext.Terms.AddRange(term1, term2);
            _dbContext.SaveChanges();

            var magazine1 = new Magazines { Title = "Magazine 1", Description = "Description 1", Faculty = faculty, Term = term1, StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-01-31") };
            var magazine2 = new Magazines { Title = "Magazine 2", Description = "Description 2", Faculty = faculty, Term = term1, StartDate = DateTime.Parse("2024-02-01"), EndDate = DateTime.Parse("2024-02-29") };
            var magazine3 = new Magazines { Title = "Magazine 3", Description = "Description 3", Faculty = faculty, Term = term2, StartDate = DateTime.Parse("2024-07-01"), EndDate = DateTime.Parse("2024-07-31") };

            _dbContext.Magazines.AddRange(magazine1, magazine2, magazine3);
            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _magazineRepository = new MagazineRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
        }

        [Test]
        public void GetAllMagazine_ReturnsAllMagazines()
        {
            // Act
            var result = _magazineRepository.GetAllMagazine();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(3, result.Count);
        }

        [Test]
        public async Task CountNumberOfMagazine_ReturnsCorrectCount()
        {
            // Arrange
            var latestTerm = _dbContext.Terms.OrderByDescending(t => t.EndDate).First();
            var expectedCount = _dbContext.Magazines.Count(m => m.TermId == latestTerm.Id);

            // Act
            var result = await _magazineRepository.CountNumberOfMagazine(1);

            // Assert
            ClassicAssert.AreEqual(expectedCount, result);
        }


        // Additional tests can be added for other methods in IMagazineRepository
    }

}
