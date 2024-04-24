using GreenwichUniversityMagazine.Areas.Admin.Controllers;
using GreenwichUniversityMagazine.Serivces.IServices;
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
    public class EmailControllerTests
    {
        private Mock<IEmailService> _mockEmailService;
        private EmailController _controller;

        [SetUp]
        public void Setup()
        {
            _mockEmailService = new Mock<IEmailService>();
            _controller = new EmailController(_mockEmailService.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }

        [Test]
        public async Task SendCode_ValidEmail_RedirectsToIndex()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            var result = await _controller.SendCode(email) as RedirectToActionResult;

            // Assert
            ClassicAssert.IsNotNull(result);
            ClassicAssert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task SendCode_ValidEmail_CallsSendEmailAsync()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            await _controller.SendCode(email);

            // Assert
            _mockEmailService.Verify(
                x => x.SendEmailAsync(
                    email,
                    It.IsAny<string>(), // Any subject
                    It.IsAny<string>() // Any message
                ),
                Times.Once
            );
        }
    }
}
