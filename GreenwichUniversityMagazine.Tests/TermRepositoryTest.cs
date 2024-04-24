using GreenwichUniversityMagazine.Data;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GreenwichUniversityMagazine.Tests
{
    [TestFixture]
    public class TermRepositoryTest
    {
        private dbContext _dbContext;
        private ITermRepository _termRepository;

        [SetUp]
        public void Setup()
        {
            // Create a mock of the database context
            var options = new DbContextOptionsBuilder<dbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new dbContext(options);

            // Add the term "Spring 2024" to the in-memory database
            _dbContext.Terms.Add(new Term { Name = "Spring 2024", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-12-31") });
            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _termRepository = new TermRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetById_ValidId_ReturnsTerm()
        {
            // Arrange
            int id = 1;

            // Act
            var term = _termRepository.GetById(id);

            // ClassicAssert
            ClassicAssert.IsNotNull(term);
            ClassicAssert.AreEqual("Spring 2024", term.Name);
            ClassicAssert.AreEqual(DateTime.Parse("2024-01-01"), term.StartDate);
            ClassicAssert.AreEqual(DateTime.Parse("2024-12-31"), term.EndDate);
        }

        [Test]
        public void GetById_InvalidId_ReturnsNull()
        {
            // Arrange
            int id = 999;

            // Act
            var term = _termRepository.GetById(id);

            // ClassicAssert
            ClassicAssert.IsNull(term);
        }

        [Test]
        public void GetAllTerm_ReturnsAllTerms()
        {
            // Act
            var terms = _termRepository.GetAllTerm();

            // ClassicAssert
            ClassicAssert.AreEqual(1, terms.Count);
            ClassicAssert.AreEqual("Spring 2024", terms.First().Name);
        }

        [Test]
        public void GetAll_WithFilter_ReturnsFilteredTerms()
        {
            // Arrange
            var expectedName = "Spring 2024";
            Expression<Func<Term, bool>> filter = term => term.Name == expectedName;

            // Act
            var terms = _termRepository.GetAll(filter);

            // Assert
            ClassicAssert.AreEqual(1, terms.Count());
            ClassicAssert.AreEqual(expectedName, terms.First().Name);
        }

        [Test]
        public void Get_WithFilter_ReturnsFilteredTerm()
        {
            // Arrange
            var expectedName = "Spring 2024";
            Expression<Func<Term, bool>> filter = term => term.Name == expectedName;

            // Act
            var term = _termRepository.Get(filter);

            // Assert
            ClassicAssert.IsNotNull(term);
            ClassicAssert.AreEqual(expectedName, term.Name);
        }

        [Test]
        public void Add_AddsNewTerm()
        {
            // Arrange
            var newTerm = new Term { Name = "Summer 2024", StartDate = DateTime.Parse("2024-06-01"), EndDate = DateTime.Parse("2024-08-31") };

            // Act
            _termRepository.Add(newTerm);
            _dbContext.SaveChanges();

            // Assert
            var addedTerm = _dbContext.Terms.FirstOrDefault(t => t.Name == "Summer 2024");
            ClassicAssert.IsNotNull(addedTerm);
        }

        [Test]
        public void Remove_RemovesTerm()
        {
            // Arrange
            var termToRemove = _dbContext.Terms.First();

            // Act
            _termRepository.Remove(termToRemove);
            _dbContext.SaveChanges();

            // Assert
            var removedTerm = _dbContext.Terms.FirstOrDefault(t => t.Id == termToRemove.Id);
            ClassicAssert.IsNull(removedTerm);
        }

        [Test]
        public void RemoveRange_RemovesMultipleTerms()
        {
            // Arrange
            var termsToRemove = _dbContext.Terms.ToList();

            // Act
            _termRepository.RemoveRange(termsToRemove);
            _dbContext.SaveChanges();

            // Assert
            var removedTerms = _dbContext.Terms.ToList();
            ClassicAssert.IsEmpty(removedTerms);
        }

        [Test]
        public void Update_UpdatesTerm()
        {
            // Arrange
            var termToUpdate = _dbContext.Terms.First();
            var newName = "Autumn 2024";
            termToUpdate.Name = newName;

            // Act
            _termRepository.Update(termToUpdate);
            _dbContext.SaveChanges();

            // Assert
            var updatedTerm = _dbContext.Terms.FirstOrDefault(t => t.Id == termToUpdate.Id);
            ClassicAssert.IsNotNull(updatedTerm);
            ClassicAssert.AreEqual(newName, updatedTerm.Name);
        }

    }
}
