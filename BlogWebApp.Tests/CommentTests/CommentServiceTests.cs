using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.UserServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogWebApp.Tests.CommentTests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<ICommentRepository> _commentRepositoryMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IArticleService> _articleServiceMock;
        private Mock<ILogger<CommentService>> _loggerMock;
        private CommentService _commentService;

        [SetUp]
        public void SetUp()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _userServiceMock = new Mock<IUserService>();
            _articleServiceMock = new Mock<IArticleService>();
            _loggerMock = new Mock<ILogger<CommentService>>();
            _commentService = new CommentService(_commentRepositoryMock.Object, _articleServiceMock.Object, _userServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldCreateComment_WhenValid()
        {
            var user = new ApplicationUser { Id = "userId" };
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _commentService.CreateCommentAsync(article.ArticleId, user.Id, "content");

            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            _userServiceMock.Verify(r => r.GetUserByIdAsync(user.Id), Times.Once);
            _articleServiceMock.Verify(s => s.GetArticleByIdAsync(article.ArticleId), Times.Once);
        }    

        [Test]
        public async Task CreateCommentAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.CreateCommentAsync(Guid.NewGuid(), "user1", "testcontent"));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _articleServiceMock
                .Setup(s => s.GetArticleByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid()));

            _userServiceMock.Setup(s => s.GetUserByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
               await _commentService.CreateCommentAsync(Guid.NewGuid(), "user1", "testcontent"));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task GetCommentById_ShouldReturnComment_WhenCommentExist()
        {
            var comment = new Comment("Test content", "testuser", Guid.NewGuid());
            var user = new ApplicationUser { Id = comment.UserId };
            var article = new Article(comment.ArticleId, "testtitle", "image", "testcontent", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync(comment);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _commentService.CreateCommentAsync(comment.ArticleId, comment.UserId, comment.Content);
            var result = await _commentService.GetCommentByIdAsync(comment.CommentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CommentId, Is.EqualTo(comment.CommentId));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
            _userServiceMock.Verify(r => r.GetUserByIdAsync(comment.UserId), Times.Once);
            _articleServiceMock.Verify(s => s.GetArticleByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetCommentById_ShouldThrowArgumentException_WhenCommentIdEmpty()
        {
            var commentId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.GetCommentByIdAsync(commentId));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(commentId), Times.Never);
        }

        [Test]
        public async Task GetCommentByIdAsync_ShouldThrowInvalidOperationException_WhenCommentNotFound()
        {
            var comment = new Comment("Test content", "1", Guid.NewGuid());

            _commentRepositoryMock.Setup(s => s.GetByIdAsync(comment.CommentId))
                .ReturnsAsync((Comment?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.GetCommentByIdAsync(comment.CommentId));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }    

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldDeleteComments_WhenCommentsExist()
        {
            var comment1 = new Comment("Test content", "1", Guid.NewGuid());
            var comment2 = new Comment("Test content", "2", comment1.ArticleId);
            var comments = new List<Comment> { comment1, comment2 };
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(comment1.ArticleId)).ReturnsAsync(comments);

            await _commentService.DeleteCommentsByArticleIdAsync(comment1.ArticleId);

            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(comments), Times.Once);
            _commentRepositoryMock.Verify(r => r.GetByArticleIdAsync(comment1.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldThrowArgumentException_WhenCommentsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
               await _commentService.DeleteCommentsByArticleIdAsync(Guid.Empty));
            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<Comment>>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentsByUserIdAsync_ShouldDeleteComments_WhenCommentsExist()
        {
            var comment1 = new Comment("Test content", "1", Guid.NewGuid());
            var comment2 = new Comment("Test content", "1", Guid.NewGuid());
            var comments = new List<Comment> { comment1, comment2 };
            _commentRepositoryMock.Setup(r => r.GetByUserIdAsync(comment1.UserId)).ReturnsAsync(comments);

            await _commentService.DeleteCommentsByUserIdAsync(comment1.UserId);

            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(comments), Times.Once);
            _commentRepositoryMock.Verify(r => r.GetByUserIdAsync(comment1.UserId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentsByUserIdAsync_ShouldThrowArgumentException_WhenCommentsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
               await _commentService.DeleteCommentsByUserIdAsync(""));
            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<Comment>>()), Times.Never);
        }
    }
}
