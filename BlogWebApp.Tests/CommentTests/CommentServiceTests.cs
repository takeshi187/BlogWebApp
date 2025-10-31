using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.UserServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BlogWebApp.Tests.CommentTests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<ICommentRepository> _commentRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IArticleService> _articleServiceMock;
        private CommentService _commentService;
        private string _guidString;

        [SetUp]
        public void Setup()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _userServiceMock = new Mock<IUserService>();
            _articleServiceMock = new Mock<IArticleService>();
            _commentService = new CommentService(_commentRepositoryMock.Object, _articleServiceMock.Object, _userServiceMock.Object);

            _guidString = "a1b2c3d4-e5f6-7890-1234-567890abcdef";
        }

        [Test]
        public async Task CreateCommentAsync_ShouldReturnComment_WhenValid()
        {
            var comment = new Comment(Guid.NewGuid(), "content", "userid", Guid.NewGuid());
            var user = new ApplicationUser { Id = comment.UserId };
            var article = new Article(comment.ArticleId, "testtitle", "image", "testcontent", Guid.Parse(_guidString));
            _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Comment>())).ReturnsAsync(comment);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            var result = await _commentService.AddCommentAsync(comment);

            Assert.That(result, Is.EqualTo(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            _userServiceMock.Verify(r => r.GetUserByIdAsync(comment.UserId), Times.Once);
            _articleServiceMock.Verify(s => s.GetArticleByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), null, "testuser", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.AddCommentAsync(comment));
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowArgumentException_WhenUserNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", null, Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.AddCommentAsync(comment));
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(comment.ArticleId))
                .ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.AddCommentAsync(comment));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldCallRepositoryDelete_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync(comment);

            await _commentService.DeleteCommentAsync(comment.CommentId);

            _commentRepositoryMock.Verify(r => r.DeleteAsync(comment), Times.Once);
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldReturnFalse_WhenCommentNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment?)null);

            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _commentService.DeleteCommentAsync(comment.CommentId));
        }
    }
}
