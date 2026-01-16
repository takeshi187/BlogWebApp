using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.Services.UserServices;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogWebApp.Tests.LikeTests
{
    [TestFixture]
    public class LikeServiceTests
    {
        private Mock<ILikeRepository> _likeRepositoryMock;
        private Mock<IArticleService> _articleServiceMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<LikeService>> _loggerMock;
        private LikeService _likeService;
        private Guid _genreId;
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            _likeRepositoryMock = new Mock<ILikeRepository>();
            _articleServiceMock = new Mock<IArticleService>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<LikeService>>();
            _likeService = new LikeService(_likeRepositoryMock.Object, _userServiceMock.Object, _articleServiceMock.Object, _loggerMock.Object);
            _genreId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
            _userId = "b1b2c3d4-e5f6-7890-1234-567890abcdef";
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldAddLike_WhenLikeDoesNotExist()
        {
            var articleId = Guid.NewGuid();
            var article = new Article(articleId, "title", "img", "content", _genreId);
            var user = new ApplicationUser { Id = _userId };

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(articleId, _userId)).ReturnsAsync((Like?)null);

            var result = await _likeService.ToggleLikeAsync(articleId, _userId);

            Assert.That(result, Is.True);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldRemoveLike_WhenLikeExists()
        {
            var articleId = Guid.NewGuid();
            var article = new Article(articleId, "title", "img", "content", _genreId);
            var user = new ApplicationUser { Id = _userId };
            var like = new Like(_userId, articleId);

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(articleId, _userId)).ReturnsAsync(like);

            var result = await _likeService.ToggleLikeAsync(articleId, _userId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(like), Times.Once);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var articleId = Guid.NewGuid();

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleId))
                .ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await _likeService.ToggleLikeAsync(articleId, _userId));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            var articleId = Guid.NewGuid();
            var article = new Article(articleId, "title", "img", "content", _genreId);

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleId))
                .ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await _likeService.ToggleLikeAsync(articleId, _userId));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.ToggleLikeAsync(Guid.NewGuid(), ""));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowArgumentException_WhenArticleIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>( async () =>
                await _likeService.ToggleLikeAsync(Guid.Empty, _userId));
        }


        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldDeleteLikesForArticle_WhenLikesExist()
        {
            var like1 = new Like(Guid.NewGuid(), "testuser", Guid.NewGuid());
            var like2 = new Like(Guid.NewGuid(), "testuser1", like1.ArticleId);
            var likes = new List<Like> { like1, like2 };
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like1.ArticleId)).ReturnsAsync(likes);

            await _likeService.DeleteLikesByArticleIdAsync(like1.ArticleId);

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(likes), Times.Once);
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(like1.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldThrowArgumentException_WhenLikesEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
               await _likeService.DeleteLikesByArticleIdAsync(Guid.Empty));
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
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
        public async Task GetLikeByIdAsync_ShouldThrowInvalidOperationException_WhenLikeNotFound()
        {
            var likeId = Guid.NewGuid();
            _likeRepositoryMock.Setup(r => r.GetByIdAsync(likeId)).ReturnsAsync((Like?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _likeService.GetLikeByIdAsync(likeId));
            _likeRepositoryMock.Verify(r => r.GetByIdAsync(likeId), Times.Once);
        }

        [Test]
        public async Task GetLikeByIdAsync_ShouldThrowArgumentException_WhenLikeEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.GetLikeByIdAsync(Guid.Empty));
            _likeRepositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Never);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldDeleteLikes_WhenLikesExist()
        {
            var likes = new List<Like>
            {
                new Like(_userId, Guid.NewGuid()),
                new Like(_userId, Guid.NewGuid())
            };

            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync(_userId))
                .ReturnsAsync(likes);
            _likeRepositoryMock.Setup(r => r.DeleteRangeAsync(likes))
                .ReturnsAsync(true);

            var result = await _likeService.DeleteLikesByUserIdAsync(_userId);

            Assert.That(result, Is.True);
            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(likes), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldReturnFalse_WhenNoLikes()
        {
            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync(_userId))
                .ReturnsAsync(new List<Like>());

            var result = await _likeService.DeleteLikesByUserIdAsync(_userId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<Like>>()), Times.Never);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async() =>
                await _likeService.DeleteLikesByUserIdAsync(""));
        }
    }
}
