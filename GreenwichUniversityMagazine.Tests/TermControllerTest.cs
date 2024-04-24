using GreenwichUniversityMagazine.Areas.Admin.Controllers;
using GreenwichUniversityMagazine.Models;
using GreenwichUniversityMagazine.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenwichUniversityMagazine.Tests
{
    [TestFixture]
    public class TermControllerTests
    {
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private TermController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _controller = new TermController(_mockUnitOfWork.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public void Index_ReturnsListOfTermsOrderedByStartDateDescending()
        {
            // Arrange
            var terms = new List<Term>
            {
                new Term { Id = 1, Name = "Spring 2024", StartDate = DateTime.Parse("2024-01-01"), EndDate = DateTime.Parse("2024-06-30") },
                new Term { Id = 2, Name = "Fall 2024", StartDate = DateTime.Parse("2024-07-01"), EndDate = DateTime.Parse("2024-12-31") }
            };
            _mockUnitOfWork.Setup(u => u.TermRepository.GetAll(null, null)).Returns(terms);

            // Act
            var result = _controller.Index() as ViewResult;
            var model = result?.Model as List<Term>;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual(2, model?.Count);
            ClassicAssert.AreEqual("Fall 2024", model?.First().Name); // Assert descending order
        }



        // Add more tests for other controller actions
    }
}
