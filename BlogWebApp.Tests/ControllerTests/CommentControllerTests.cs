using BlogWebApp.Controllers;
using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class CommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private CommentController _controller;

        private const string UserId = "user-1";

        [SetUp]
        public void SetUp()
        {
            _commentServiceMock = new Mock<ICommentService>();
            _controller = new CommentController(null!, _commentServiceMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            }, "TestAuth"));

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task AddComment_ShouldAddComment_WhenValid()
        {
            var articleId = Guid.NewGuid();

            _commentServiceMock
                .Setup(s => s.CreateCommentAsync(articleId, UserId, "comment"));

            var result = await _controller.AddComment(articleId, "comment");

            _commentServiceMock.Verify(
                s => s.CreateCommentAsync(articleId, UserId, "comment"),
                Times.Once);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
        }

        [Test]
        public async Task AddComment_ShouldRedirectWithError_WhenContentEmpty()
        {
            var articleId = Guid.NewGuid();

            var result = await _controller.AddComment(articleId, "");

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Article"));
            Assert.That(redirect.RouteValues["id"], Is.EqualTo(articleId));

            Assert.That(_controller.TempData["Error"],
                Is.EqualTo("Комментарий не может быть пустым."));

            _commentServiceMock.Verify(
                s => s.CreateCommentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Test]
        public async Task AddComment_ShouldThrowsException_WhenServiceError()
        {
            var articleId = Guid.NewGuid();

            _commentServiceMock
                .Setup(s => s.CreateCommentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("db error"));

            var result = await _controller.AddComment(articleId, "comment");

            Assert.That(_controller.TempData["Error"],
                Is.EqualTo("Не удалось добавить комментарий."));

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller != null)
            {
                _controller.Dispose();
            }
        }
    }
}
