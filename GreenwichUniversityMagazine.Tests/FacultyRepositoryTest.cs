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
    public class FacultyRepositoryTest
    {
        private dbContext _dbContext;
        private IFacultyRepository _facultyRepository;

        [SetUp]
        public void Setup()
        {
            // Create a mock of the database context
            var options = new DbContextOptionsBuilder<dbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new dbContext(options);

            // Add sample data to the in-memory database
            _dbContext.Faculties.AddRange(
                new Faculty { Id = 1, Name = "Faculty 1" },
                new Faculty { Id = 2, Name = "Faculty 2" },
                new Faculty { Id = 3, Name = "Faculty 3" }
            );
            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _facultyRepository = new FacultyRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void GetAllFaculty_ReturnsAllFaculties()
        {
            // Act
            var result = _facultyRepository.GetAllFaculty();

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(3, result.Count); // Adjust the count based on the sample data
        }

        [Test]
        public void GetNumbersOfItems_ReturnsCountOfItemsForGivenUser()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = _facultyRepository.GetNumbersOfItems(userId);

            // Assert
            ClassicAssert.AreEqual(1, result); // Adjust the count based on the sample data
        }



        [Test]
        public void Update_ExistingFaculty_UpdatesFacultyInDatabase()
        {
            // Arrange
            int facultyId = 1;
            string updatedName = "Updated Faculty";

            // Act
            var existingFaculty = _dbContext.Faculties.FirstOrDefault(f => f.Id == facultyId);
            existingFaculty.Name = updatedName;
            _facultyRepository.Update(existingFaculty);
            _dbContext.SaveChanges(); // Save changes to ensure the faculty is updated in the database

            // Assert
            var updatedFaculty = _dbContext.Faculties.FirstOrDefault(f => f.Id == facultyId);
            ClassicAssert.IsNotNull(updatedFaculty);
            ClassicAssert.AreEqual(updatedName, updatedFaculty.Name);
        }
    }

}
