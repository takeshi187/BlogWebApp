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

        [SetUp]
        public void SetUp()
        {
            _likeRepositoryMock = new Mock<ILikeRepository>();
            _articleServiceMock = new Mock<IArticleService>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<LikeService>>();
            _likeService = new LikeService(_likeRepositoryMock.Object, _userServiceMock.Object, _articleServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldAddLike_WhenLikeDoesNotExist()
        {
            var article = new Article(Guid.NewGuid(), "title", "img", "content", Guid.NewGuid());
            var user = new ApplicationUser { Id = "user1" };

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync("user1")).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, "user1")).ReturnsAsync((Like?)null);

            await _likeService.ToggleLikeAsync(article.ArticleId, "user1");

            _likeRepositoryMock.Verify(r => r.ExistAsync(article.ArticleId, "user1"), Times.Once);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldRemoveLike_WhenLikeExists()
        {
            var article = new Article(Guid.NewGuid(), "title", "img", "content", Guid.NewGuid());
            var user = new ApplicationUser { Id = "user1" };
            var like = new Like("user1", article.ArticleId);

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync("user1")).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, "user1")).ReturnsAsync(like);

            await _likeService.ToggleLikeAsync(article.ArticleId, "user1");

            _likeRepositoryMock.Verify(r => r.ExistAsync(article.ArticleId, "user1"), Times.Once);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(like), Times.Once);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await _likeService.ToggleLikeAsync(Guid.NewGuid(), "user1"));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            var article = new Article(Guid.NewGuid(), "title", "img", "content", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId))
                .ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync("user1"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async() =>
                await _likeService.ToggleLikeAsync(article.ArticleId, "user1"));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenUserIdIsEmpty()
        {
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _likeService.ToggleLikeAsync(Guid.NewGuid(), ""));
        }

        [Test]
        public async Task ToggleLikeAsync_ShouldThrowInvalidOperationException_WhenArticleIdIsEmpty()
        {
            Assert.ThrowsAsync<InvalidOperationException>( async () =>
                await _likeService.ToggleLikeAsync(Guid.Empty, "user1"));
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
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(articleId))
                .ReturnsAsync(new List<Like>());

            await _likeService.DeleteLikesByArticleIdAsync(articleId);

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<Like>>()), Times.Once);
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(It.IsAny<Guid>()), Times.Once);
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
                new Like("user1", Guid.NewGuid()),
                new Like("user1", Guid.NewGuid())
            };

            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync("user1"))
                .ReturnsAsync(likes);
            _likeRepositoryMock.Setup(r => r.DeleteRangeAsync(likes));

            await _likeService.DeleteLikesByUserIdAsync("user1");

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(likes), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldReturnFalse_WhenNoLikes()
        {
            _likeRepositoryMock.Setup(r => r.GetByUserIdAsync("user1"))
                .ReturnsAsync(new List<Like>());

            await _likeService.DeleteLikesByUserIdAsync("user1");

            _likeRepositoryMock.Verify(r => r.DeleteRangeAsync(It.IsAny<IEnumerable<Like>>()), Times.Once);
            _likeRepositoryMock.Verify(r => r.GetByUserIdAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldThrow_WhenUserIdIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async() =>
                await _likeService.DeleteLikesByUserIdAsync(""));
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }
    }
}
