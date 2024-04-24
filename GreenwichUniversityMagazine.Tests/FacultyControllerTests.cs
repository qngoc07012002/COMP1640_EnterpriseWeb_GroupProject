using GreenwichUniversityMagazine.Areas.Admin.Controllers;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
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
    public class FacultyControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private FacultyController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new FacultyController(_mockUnitOfWork.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public void Index_ReturnsListOfFaculties()
        {
            // Arrange
            var faculties = new List<Faculty>
            {
                new Faculty { Id = 1, Name = "Faculty 1" },
                new Faculty { Id = 2, Name = "Faculty 2" }
            };
            _mockUnitOfWork.Setup(u => u.FacultyRepository.GetAll(null, null)).Returns(faculties);

            // Act
            var result = _controller.Index() as ViewResult;
            var model = result?.Model as List<Faculty>;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(faculties.Count, model?.Count);
        }

        [Test]
        public void Create_GET_ReturnsView()
        {
            // Act
            var result = _controller.Create() as ViewResult;

            // Assert
            ClassicAssert.IsNotNull(result);
        }



        [Test]
        public void Edit_GET_WithValidId_ReturnsView()
        {
            // Arrange
            var faculty = new Faculty { Id = 1, Name = "Test Faculty" };
            _mockUnitOfWork.Setup(u => u.FacultyRepository.Get(It.IsAny<Expression<Func<Faculty, bool>>>(), null, false)).Returns(faculty);

            // Act
            var result = _controller.Edit(1) as ViewResult;

            // Assert
            ClassicAssert.IsNotNull(result);
        }





        [Test]
        public void Delete_GET_WithValidId_ReturnsView()
        {
            // Arrange
            var faculty = new Faculty { Id = 1, Name = "Test Faculty" };
            _mockUnitOfWork.Setup(u => u.FacultyRepository.Get(It.IsAny<Expression<Func<Faculty, bool>>>(), null, false)).Returns(faculty);

            // Act
            var result = _controller.Delete(1) as ViewResult;

            // Assert
            ClassicAssert.IsNotNull(result);
        }


    }
}
