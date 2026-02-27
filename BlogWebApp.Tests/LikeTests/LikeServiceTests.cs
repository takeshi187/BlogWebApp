using BlogWebApp.Models;
using BlogWebApp.Services.LikeServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogWebApp.Tests.LikeTests
{
    [TestFixture]
    public class LikeServiceTests
    {
        private Mock<ILikeRepository> _likeRepositoryMock;
        private Mock<ILogger<LikeService>> _loggerMock;
        private LikeService _likeService;

        [SetUp]
        public void SetUp()
        {
            _likeRepositoryMock = new Mock<ILikeRepository>();
            _loggerMock = new Mock<ILogger<LikeService>>();
            _likeService = new LikeService(_likeRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldAddLike_WhenLikeDoesNotExist()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAndUserIdAsync(articleId, userId)).ReturnsAsync((Like?)null);

            await _likeService.ToggleLikeAsync(articleId, userId);

            _likeRepositoryMock.Verify(r => r.GetByArticleIdAndUserIdAsync(articleId, userId), Times.Once);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldDeleteLike_WhenLikeExists()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";
            var existingLike = new Like(userId, articleId);
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAndUserIdAsync(articleId, userId)).ReturnsAsync(existingLike);

            await _likeService.ToggleLikeAsync(articleId, userId);

            _likeRepositoryMock.Verify(r => r.GetByArticleIdAndUserIdAsync(articleId, userId), Times.Once);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(existingLike), Times.Once);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowDbUpdateException_WhenRaceCondition()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";

            _likeRepositoryMock.Setup(r => r.GetByArticleIdAndUserIdAsync(articleId, userId)).ReturnsAsync((Like?)null);
            var dbException = new DbUpdateException("Unique constraint violation");
            _likeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Like>())).ThrowsAsync(dbException);

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _likeService.ToggleLikeAsync(articleId, userId));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
        }

        [Test]
        public void ToggleLikeAsync_ShouldThrowArgumentException_WhenUserIdInvalid()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.ToggleLikeAsync(Guid.NewGuid(), ""));
            _likeRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void ToggleLikeAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.ToggleLikeAsync(Guid.Empty, "user"));
            _likeRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void ToggleLikeAsync_ShouldThrowDbUpdateException_WhenUnexpectedError()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAndUserIdAsync(articleId, userId)).ReturnsAsync((Like?)null);
            var dbException = new DbUpdateException("Unexpected error");
            _likeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Like>())).ThrowsAsync(dbException);

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _likeService.ToggleLikeAsync(articleId, userId));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
        }

        [Test]
        public void ToggleLikeAsync_ShouldThrow_WhenUnexpectedDbError()
        {
            var articleId = Guid.NewGuid();
            var userId = "user1";
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAndUserIdAsync(articleId, userId)).ReturnsAsync((Like?)null);
            var dbException = new DbUpdateException("Some db error");
            _likeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Like>())).ThrowsAsync(dbException);

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _likeService.ToggleLikeAsync(articleId, userId));

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    dbException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetLikeByIdAsync_ShouldReturnLike_WhenLikeExist()
        {
            var likeId = Guid.NewGuid();
            var like = new Like("user1", likeId);
            _likeRepositoryMock.Setup(r => r.GetByIdAsync(likeId)).ReturnsAsync(like);

            var result = await _likeService.GetLikeByIdAsync(likeId);

            Assert.That(result, Is.EqualTo(like));
            _likeRepositoryMock.Verify(r => r.GetByIdAsync(likeId), Times.Once);
        }
        
        [Test]
        public async Task GetLikeByIdAsync_ShouldReturnNull_WhenLikeNotFound()
        {
            var likeId = Guid.NewGuid();
            _likeRepositoryMock.Setup(r => r.GetByIdAsync(likeId)).ReturnsAsync((Like?)null);

            var result = await _likeService.GetLikeByIdAsync(likeId);

            Assert.That(result, Is.Null);
            _likeRepositoryMock.Verify(r => r.GetByIdAsync(likeId), Times.Once);
        }

        [Test]
        public async Task GetLikeByIdAsync_ShouldThrowArgumentException_WhenLikeIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.GetLikeByIdAsync(Guid.Empty));
            _likeRepositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Never);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldDeleteLikesForArticle_WhenLikesExist()
        {
            var like1 = new Like("testuser", Guid.NewGuid());
            var like2 = new Like("testuser1", like1.ArticleId);
            var likes = new List<Like> { like1, like2 };
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like1.ArticleId)).ReturnsAsync(likes);

            await _likeService.DeleteLikesByArticleIdAsync(like1.ArticleId);

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(likes), Times.Once);
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(like1.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldReturnFalse_WhenNoLikes()
        {
            var articleId = Guid.NewGuid();
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(articleId)).ReturnsAsync(new List<Like>());

            var result = await _likeService.DeleteLikesByArticleIdAsync(articleId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IReadOnlyCollection<Like>>()), Times.Never);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldThrowArgumentException_WhenLikesEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
               await _likeService.DeleteLikesByArticleIdAsync(Guid.Empty));
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public void DeleteLikesByArticleIdAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var articleId = Guid.NewGuid();
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(articleId)).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _likeService.DeleteLikesByArticleIdAsync(articleId));
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(articleId), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldDeleteLikes_WhenLikesExist()
        {
            var likes = new List<Like>
            {
                new Like("user1", Guid.NewGuid()),
                new Like("user1", Guid.NewGuid())
            };

            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync("user1")).ReturnsAsync(likes);
            _likeRepositoryMock.Setup(r => r.DeleteRangeAsync(likes));

            await _likeService.DeleteLikesByUserIdAsync("user1");

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(likes), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldReturnFalse_WhenNoLikes()
        {
            var userId = "user1";

            var result = await _likeService.DeleteLikesByUserIdAsync(userId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IReadOnlyCollection<Like>>()), Times.Never);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.DeleteLikesByUserIdAsync(""));
            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IReadOnlyCollection<Like>>()), Times.Never);
        }

        [Test]
        public void DeleteLikesByUserIdAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var userId = "test";
            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync("test")).ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _likeService.DeleteLikesByUserIdAsync("test"));
            _likeRepositoryMock.Verify(r => r.GetByUserIdAsync("test"), Times.Once);
        }
    }
}
