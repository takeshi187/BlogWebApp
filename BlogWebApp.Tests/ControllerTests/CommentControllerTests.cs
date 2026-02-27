using BlogWebApp.Controllers;
using BlogWebApp.Services.CommentServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Security.Claims;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class CommentControllerTests
    {
        private Mock<ICommentService> _commentServiceMock;
        private CommentController _commentController;

        private const string UserId = "user-1";

        [SetUp]
        public void SetUp()
        {
            _commentServiceMock = new Mock<ICommentService>();

            _commentController = new CommentController(_commentServiceMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            }, "TestAuth"));

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            _commentController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _commentController.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task AddCommentPost_ShouldAddComment_WhenValid()
        {
            var articleId = Guid.NewGuid();
            var commentContent = "This is a valid comment";

            _commentServiceMock.Setup(s => s.CreateCommentAsync(articleId, UserId, commentContent))
                .Returns(Task.CompletedTask);

            var result = await _commentController.AddComment(articleId, commentContent);

            _commentServiceMock.Verify(s => s.CreateCommentAsync(articleId, UserId, commentContent), Times.Once);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Article"));
            Assert.That(redirect.RouteValues["id"], Is.EqualTo(articleId));
        }

        [Test]
        public async Task AddCommentPost_ShouldRedirectWithError_WhenContentEmpty()
        {
            var articleId = Guid.NewGuid();
            var commentContent = "";

            var result = await _commentController.AddComment(articleId, commentContent);

            _commentServiceMock.Verify(s => s.CreateCommentAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Details"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Article"));
            Assert.That(redirect.RouteValues["id"], Is.EqualTo(articleId));
            Assert.That(_commentController.TempData["CommentError"], Is.EqualTo("Комментарий не может быть пустым."));
        }

        [Test]
        public async Task AddCommentPost_ShouldRedirectWithError_WhenDbException()
        {
            var articleId = Guid.NewGuid();
            var commentContent = "This is a invalid comment";
            _commentServiceMock.Setup(s => s.CreateCommentAsync(articleId, UserId, commentContent))
                .ThrowsAsync(new Exception("Database error"));

            Assert.ThrowsAsync<Exception>(() => _commentController.AddComment(articleId, commentContent));
        }

        [Test]
        public async Task DeleteCommentPost_ShouldDeleteCommentAndRedirectToDetails_WhenValid()
        {
            var commentId = Guid.NewGuid();
            var articleId = Guid.NewGuid();
            _commentServiceMock.Setup(s => s.DeleteCommentByIdAsync(commentId)).ReturnsAsync(true);

            var result = await _commentController.Delete(commentId, articleId);
            var redirectResult = result as RedirectToActionResult;

            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Details"));
            Assert.That(redirectResult.ControllerName, Is.EqualTo("Article"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(articleId));
            _commentServiceMock.Verify(s => s.DeleteCommentByIdAsync(commentId), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            if (_commentController != null)
            {
                _commentController.Dispose();
            }
        }
    }
}
