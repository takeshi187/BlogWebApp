using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.Design;

namespace BlogWebApp.Tests.CommentTests
{
    [TestFixture]
    public class CommentServiceTests
    {
        private Mock<ICommentRepository> _commentRepositoryMock;
        private Mock<ILogger<CommentService>> _loggerMock;
        private CommentService _commentService;

        [SetUp]
        public void SetUp()
        {
            _commentRepositoryMock = new Mock<ICommentRepository>();
            _loggerMock = new Mock<ILogger<CommentService>>();
            _commentService = new CommentService(_commentRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateCommentAsync_ShouldCreateComment_WhenValid()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";
            var content = "testcomment";

            await _commentService.CreateCommentAsync(articleId, userId, content);

            _commentRepositoryMock.Verify(r => r.AddAsync(It.Is<Comment>(c =>
                    c.ArticleId == articleId &&
                    c.UserId == userId &&
                    c.Content == content)),
                    Times.Once);
        }

        [Test]
        public void CreateCommentAsync_ShouldThrowArgumentException_WhenInvalidData()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.CreateCommentAsync(Guid.Empty, "", ""));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public void CreateCommentAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";

            _commentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Comment>())).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _commentService.CreateCommentAsync(articleId, userId, "content"));
            _commentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Test]
        public async Task GetCommentById_ShouldReturnComment_WhenCommentExist()
        {
            var comment = new Comment("content", "user1", Guid.NewGuid());

            _commentRepositoryMock
                .Setup(r => r.GetByIdAsync(comment.CommentId))
                .ReturnsAsync(comment);

            var result = await _commentService.GetCommentByIdAsync(comment.CommentId);

            Assert.That(result, Is.EqualTo(comment));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(comment.CommentId), Times.Once);
        }

        [Test]
        public void GetCommentByIdAsync_ShouldThrowArgumentException_WhenCommentIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.GetCommentByIdAsync(Guid.Empty));
            _commentRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task GetCommentByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var commentId = Guid.NewGuid();
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(commentId)).ReturnsAsync((Comment?)null);

            var result = await _commentService.GetCommentByIdAsync(commentId);
            Assert.That(result, Is.Null);
            _commentRepositoryMock.Verify(s => s.GetByIdAsync(commentId), Times.Once);
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
        public void DeleteCommentsByArticleIdAsync_ShouldThrowArgumentException_WhenIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.DeleteCommentsByArticleIdAsync(Guid.Empty));
            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IReadOnlyCollection<Comment>>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldReturnFalse_WhenNull()
        {
            var commentId = Guid.NewGuid();
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(commentId)).ReturnsAsync((List<Comment>?)null);

            var result = await _commentService.DeleteCommentsByArticleIdAsync(commentId);

            Assert.That(result, Is.False);
            _commentRepositoryMock.Verify(s => s.GetByArticleIdAsync(commentId), Times.Once);
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldReturnFalse_WhenCommentsEmpty()
        {
            var commentId = Guid.NewGuid();
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(commentId)).ReturnsAsync(new List<Comment>());

            var result = await _commentService.DeleteCommentsByArticleIdAsync(commentId);
            Assert.That(result, Is.False);
            _commentRepositoryMock.Verify(s => s.GetByArticleIdAsync(commentId), Times.Once);
        }

        [Test]
        public void DeleteCommentsByArticleIdAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var id = Guid.NewGuid();
            var comments = new List<Comment>{ new Comment("content", "user", id) };
            _commentRepositoryMock.Setup(r => r.GetByArticleIdAsync(id)).ReturnsAsync(comments);
            _commentRepositoryMock.Setup(r => r.DeleteRangeAsync(comments)).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () => await _commentService.DeleteCommentsByArticleIdAsync(id));
            _commentRepositoryMock.Verify(s => s.DeleteRangeAsync(comments), Times.Once);
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
        public void DeleteCommentsByUserIdAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.DeleteCommentsByUserIdAsync(""));
            _commentRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IReadOnlyCollection<Comment>>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentsByUserIdAsync_ShouldReturnFalse_WhenNull()
        {
            _commentRepositoryMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync((List<Comment>?)null);

            var result = await _commentService.DeleteCommentsByUserIdAsync("user1");

            Assert.That(result, Is.False);
            _commentRepositoryMock.Verify(s => s.GetByUserIdAsync("user1"), Times.Once);
        }

        [Test]
        public async Task DeleteCommentsByUserIdAsync_ShouldReturnFalse_WhenCommentsEmpty()
        {
            _commentRepositoryMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(new List<Comment>());

            var result = await _commentService.DeleteCommentsByUserIdAsync("user1");

            Assert.That(result, Is.False);
            _commentRepositoryMock.Verify(s => s.GetByUserIdAsync("user1"), Times.Once);
        }

        [Test]
        public void DeleteCommentsByUserIdAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var comments = new List<Comment>{ new Comment("c", "user1", Guid.NewGuid()) };
            _commentRepositoryMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(comments);
            _commentRepositoryMock.Setup(r => r.DeleteRangeAsync(comments)).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _commentService.DeleteCommentsByUserIdAsync("user1"));
            _commentRepositoryMock.Verify(s => s.DeleteRangeAsync(comments), Times.Once);
        }

        [Test]
        public void DeleteCommentByIdAsync_ShouldThrowArgumentException_WhenIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _commentService.DeleteCommentByIdAsync(Guid.Empty));
            _commentRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Test]
        public async Task DeleteCommentByIdAsync_ShouldReturnFalse_WhenNotFound()
        {
            var comment = new Comment("Test content", "1", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync((Comment?)null);

            var result = await _commentService.DeleteCommentByIdAsync(comment.CommentId);

            Assert.That(result, Is.False);
            _commentRepositoryMock.Verify(s => s.DeleteAsync(comment), Times.Never);
        }

        [Test]
        public void DeleteCommentByIdAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var comment = new Comment("content", "user", Guid.NewGuid());
            _commentRepositoryMock.Setup(r => r.GetByIdAsync(comment.CommentId)).ReturnsAsync(comment);
            _commentRepositoryMock.Setup(r => r.DeleteAsync(comment)).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _commentService.DeleteCommentByIdAsync(comment.CommentId));
            _commentRepositoryMock.Verify(s => s.DeleteAsync(comment), Times.Once);
        }
    }
}
