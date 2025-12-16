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
        public void Setup()
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
            var comment = new Comment(Guid.NewGuid(), "content", "userid", Guid.NewGuid());
            var user = new ApplicationUser { Id = comment.UserId };
            var article = new Article(comment.ArticleId, "testtitle", "image", "testcontent", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Comment>())).ReturnsAsync(comment);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            var result = await _commentService.CreateCommentAsync(comment);

            Assert.That(result, Is.EqualTo(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
            _userServiceMock.Verify(r => r.GetUserByIdAsync(comment.UserId), Times.Once);
            _articleServiceMock.Verify(s => s.GetArticleByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "", "testuser", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.CreateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.CreateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "1", Guid.Empty);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.CreateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(comment.ArticleId))
                .ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.CreateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());

            _userServiceMock.Setup(s => s.GetUserByIdAsync(comment.UserId))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.CreateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task GetCommentById_ShouldReturnComment_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());
            var user = new ApplicationUser { Id = comment.UserId };
            var article = new Article(comment.ArticleId, "testtitle", "image", "testcontent", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync(comment);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _commentService.CreateCommentAsync(comment);
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
            var comment = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());

            _commentRepositoryMock.Setup(s => s.GetByIdAsync(comment.CommentId))
                .ReturnsAsync((Comment?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.GetCommentByIdAsync(comment.CommentId));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }

        [Test]
        public async Task GetCommentsByArticleIdAsync_ShouldReturnComments_WhenCommentsExist()
        {
            var comment1 = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());
            var comment2 = new Comment(Guid.NewGuid(), "Test content", "2", comment1.ArticleId);
            var comments = new List<Comment> { comment1, comment2 };
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(comment1.ArticleId)).ReturnsAsync(comments);

            var result = await _commentService.GetCommentsByArticleIdAsync(comment1.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            _commentRepositoryMock.Verify(r => r.GetByArticleIdAsync(comment1.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetCommentsByArticleIdAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            var articleId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.GetCommentsByArticleIdAsync(articleId));
            _commentRepositoryMock.Verify(r => r.GetByArticleIdAsync(articleId), Times.Never);
        }

        [Test]
        public async Task GetCommentByArticleIdAsync_ShouldThrowInvalidOperationException_WhenCommentsNotFound()
        {
            var articleId = Guid.NewGuid();
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(articleId))
                .ReturnsAsync(new List<Comment>());

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.GetCommentsByArticleIdAsync(articleId));
            _commentRepositoryMock.Verify(r => r.GetByArticleIdAsync(articleId), Times.Once);
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldUpdateComment_WhenCommentExist()
        {
            var existingComment = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());
            var user = new ApplicationUser { Id = existingComment.UserId };
            var article = new Article(existingComment.ArticleId, "testtitle", "image", "testcontent", Guid.NewGuid());
            var updatedComment = new Comment(existingComment.CommentId, "Updated", "1", existingComment.ArticleId);
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(existingComment.CommentId)).ReturnsAsync(existingComment);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(user.Id)).ReturnsAsync(user);
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            var result = await _commentService.UpdateCommentAsync(updatedComment);

            _commentRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Comment>(a =>
                a.CommentId == existingComment.CommentId &&
                a.Content == "Updated" &&
                a.UserId == updatedComment.UserId &&
                a.ArticleId == updatedComment.ArticleId &&
                a.UpdatedAt.HasValue
                )), Times.Once);
            Assert.That(result, Is.EqualTo(existingComment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(existingComment.CommentId), Times.Once);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "", "1", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.UpdateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Never);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "test content", "", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.UpdateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Never);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            var comment = new Comment(Guid.NewGuid(), "test content", "1", Guid.Empty);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.UpdateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Never);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowArgumentException_WhenCommentEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.UpdateCommentAsync(null));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Never);
        }

        [Test]
        public void UpdateCommentAsync_ShouldThrowInvalidOperationException_WhenCommentNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());

            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync((Comment?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _commentService.UpdateCommentAsync(comment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldDeleteCommentsForArticle_WhenCommentsExist()
        {
            var comment1 = new Comment(Guid.NewGuid(), "Test content", "1", Guid.NewGuid());
            var comment2 = new Comment(Guid.NewGuid(), "Test content", "2", comment1.ArticleId);
            var comments = new List<Comment> { comment1, comment2};
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
            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldDeleteComment_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync(comment);

            await _commentService.DeleteCommentAsync(comment.CommentId);

            _commentRepositoryMock.Verify(r => r.DeleteAsync(comment), Times.Once);
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldThrowInvalidOperationException_WhenCommentNotFound()
        {
            var comment = new Comment(Guid.NewGuid(), "Test content", "testuser", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Comment?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _commentService.DeleteCommentAsync(comment.CommentId));
            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldThrowArgumentException_WhenCommentEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
               await _commentService.DeleteCommentAsync(Guid.Empty));
            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
        }
    }
}
