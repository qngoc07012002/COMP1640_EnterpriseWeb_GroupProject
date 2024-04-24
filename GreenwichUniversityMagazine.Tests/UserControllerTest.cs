using GreenwichUniversityMagazine.Areas.Admin.Controllers;
using GreenwichUniversityMagazine.Models.ViewModels;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using GreenwichUniversityMagazine.Serivces.IServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GreenwichUniversityMagazine.Tests
{
    [TestFixture]
    public class UserControllerTest
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private Mock<IEmailService> _mockEmailService;
        private UserController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockEmailService = new Mock<IEmailService>();
            _controller = new UserController(_mockUnitOfWork.Object, _mockWebHostEnvironment.Object, _mockEmailService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public void Index_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = 1, Name = "User 1" },
            new User { Id = 2, Name = "User 2" }
        };
            _mockUnitOfWork.Setup(u => u.UserRepository.GetAll(null, null)).Returns(users);

            // Act
            var result = _controller.Index() as ViewResult;
            var model = result?.Model as List<User>;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(users.Count, model?.Count);
        }

      
 
    }

}
