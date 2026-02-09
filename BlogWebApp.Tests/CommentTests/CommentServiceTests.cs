using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using Microsoft.Extensions.Logging;
using Moq;

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
    }
}
