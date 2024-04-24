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
    public class UserRepositoryTest
    {
        private dbContext _dbContext;
        private IUserRepository _userRepository;

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
            var user1 = new User { Email = "user1@example.com", Password = "password1", Name = "User 1", Faculty = faculty, Role = "STUDENT", Status = true };
            var user2 = new User { Email = "user2@example.com", Password = "password2", Name = "User 2", Faculty = faculty, Role = "STUDENT", Status = true };
            var adminUser = new User { Email = "admin@example.com", Password = "adminpassword", Name = "Admin User", Role = "ADMIN", Status = true };

            _dbContext.Faculties.Add(faculty);
            _dbContext.Users.AddRange(user1, user2, adminUser);
            _dbContext.SaveChanges();

            // Initialize the repository with the mock context
            _userRepository = new UserRepository(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database after each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void CheckEmail_NewUser_ReturnsTrue()
        {
            // Arrange
            var newUser = new User { Email = "newuser@example.com" };

            // Act
            var result = _userRepository.CheckEmail(newUser);

            // Assert
            ClassicAssert.IsTrue(result);
        }

        [Test]
        public void CheckEmail_ExistingUser_ReturnsFalse()
        {
            // Arrange
            var existingUser = new User { Email = "user1@example.com" };

            // Act
            var result = _userRepository.CheckEmail(existingUser);

            // Assert
            ClassicAssert.IsFalse(result);
        }

        [Test]
        public void CheckPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var password = "password1";

            // Act
            var result = _userRepository.CheckPassword(userId, password);

            // Assert
            ClassicAssert.IsTrue(result);
        }

        [Test]
        public void CheckPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var userId = 1;
            var incorrectPassword = "incorrectpassword";

            // Act
            var result = _userRepository.CheckPassword(userId, incorrectPassword);

            // Assert
            ClassicAssert.IsFalse(result);
        }

        [Test]
        public void Login_ExistingUser_ReturnsUser()
        {
            // Arrange
            var email = "user2@example.com";
            var password = "password2";

            // Act
            var result = _userRepository.Login(email, password);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(email, result.Email);
        }

        [Test]
        public void Register_NewUser_AddsUserToDatabase()
        {
            // Arrange
            var newUser = new User { Email = "newuser@example.com", Password = "password" };

            // Act
            _userRepository.Register(newUser);

            // Assert
            var addedUser = _dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
            ClassicAssert.IsNotNull(addedUser);
        }

        [Test]
        public void GetById_ValidId_ReturnsUser()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = _userRepository.GetById(userId);

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(userId, result.Id);
        }

        [Test]
        public void GetById_InvalidId_ReturnsNull()
        {
            // Act
            var result = _userRepository.GetById(999);

            // Assert
            ClassicAssert.IsNull(result);
        }

        [Test]
        public void GetNumberOfStudents_ReturnsCorrectCount()
        {
            // Act
            var result = _userRepository.GetNumberOfStudents();

            // Assert
            ClassicAssert.AreEqual(2, result);
        }

        [Test]
        public void Add_NewUser_AddsUserToDatabase()
        {
            // Arrange
            var newUser = new User
            {
                Email = "newuser@example.com",
                Password = "password",
                Name = "New User",
                Code = "12345",
                DateOfBirth = DateTime.Parse("2000-01-01"),
                FacultyId = 1,
                avtUrl = "avatar-url",
                Role = "STUDENT",
                Status = true
            };

            // Act
            _userRepository.Add(newUser);
            _dbContext.SaveChanges(); // Save changes to ensure user is added to the database

            // Assert
            var addedUser = _dbContext.Users.FirstOrDefault(u => u.Email == newUser.Email);
            ClassicAssert.IsNotNull(addedUser);
        }

        [Test]
        public void Remove_ExistingUser_RemovesUserFromDatabase()
        {
            // Arrange
            var userIdToRemove = 1;

            // Act
            var userToRemove = _dbContext.Users.FirstOrDefault(u => u.Id == userIdToRemove);
            _userRepository.Remove(userToRemove);
            _dbContext.SaveChanges(); // Save changes to ensure user is removed from the database

            // Assert
            var removedUser = _dbContext.Users.FirstOrDefault(u => u.Id == userIdToRemove);
            ClassicAssert.IsNull(removedUser);
        }

        [Test]
        public void RemoveRange_MultipleUsers_RemovesUsersFromDatabase()
        {
            // Act
            var usersToRemove = _dbContext.Users.ToList();
            _userRepository.RemoveRange(usersToRemove);
            _dbContext.SaveChanges(); // Save changes to ensure users are removed from the database

            // Assert
            var remainingUsersCount = _dbContext.Users.Count();
            ClassicAssert.AreEqual(0, remainingUsersCount);
        }


        [Test]
        public void Update_UserUpdatesSuccessfully()
        {
            // Arrange
            var userToUpdate = _dbContext.Users.First();
            var newName = "Updated Name";
            userToUpdate.Name = newName;

            // Act
            _userRepository.Update(userToUpdate);

            // Assert
            var updatedUser = _dbContext.Users.FirstOrDefault(u => u.Id == userToUpdate.Id);
            ClassicAssert.IsNotNull(updatedUser);
            ClassicAssert.AreEqual(newName, updatedUser.Name);
        }

    }

}