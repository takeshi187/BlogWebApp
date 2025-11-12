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
        public async Task AddLikeAsync_ShouldAddLike_WhenValid()
        {
            var article = new Article(Guid.NewGuid(), "title", "testimage", "testcontent", _genreId);
            var user = new ApplicationUser { Id = _userId };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, _userId)).ReturnsAsync(false);

            var result = await _likeService.AddLikeAsync(article.ArticleId, _userId);

            Assert.That(result, Is.True);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Once);
        }

        [Test]
        public async Task AddLikeAsync_ShouldReturnFalse_WhenAlreadyLiked()
        {
            var article = new Article(Guid.NewGuid(), "title", "testimage", "testcontent", _genreId);
            var user = new ApplicationUser { Id = _userId };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, _userId)).ReturnsAsync(true);

            var result = await _likeService.AddLikeAsync(article.ArticleId, _userId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task AddLikeAsync_ShouldThrowArgumentException_WhenUserIdIsEmpty()
        {
            var article = new Article(Guid.NewGuid(), "title", "testimage", "testcontent", _genreId);
            var user = new ApplicationUser { Id = string.Empty };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.AddLikeAsync(article.ArticleId, user.Id));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task AddLikeAsync_ShouldThrowArgumentException_WhenArticleIdIsEmpty()
        {
            var user = new ApplicationUser { Id = _userId };
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _likeService.AddLikeAsync(Guid.Empty, user.Id));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task AddLikeAsync_ShouldThrowInvalidOperation_WhenArticleIdNotFound()
        {
            var article = new Article(Guid.NewGuid(), "title", "testimage", "testcontent", Guid.NewGuid());
            var user = new ApplicationUser { Id = _userId };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync((Article?)null);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync(user);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, _userId)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _likeService.AddLikeAsync(article.ArticleId, user.Id));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task AddLikeAsync_ShouldThrowInvalidOperation_WhenUserIdNotFound()
        {
            var article = new Article(Guid.NewGuid(), "title", "testimage", "testcontent", Guid.NewGuid());
            var user = new ApplicationUser { Id = _userId };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _userServiceMock.Setup(s => s.GetUserByIdAsync(_userId)).ReturnsAsync((ApplicationUser?)null);
            _likeRepositoryMock.Setup(r => r.ExistAsync(article.ArticleId, _userId)).ReturnsAsync(false);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _likeService.AddLikeAsync(article.ArticleId, user.Id));
            _likeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task DeleteLikeAsync_ShouldDeleteLike_WhenLikeExist()
        {
            var like = new Like(_userId, Guid.NewGuid());
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like.ArticleId)).ReturnsAsync(new List<Like> { like });

            var result = await _likeService.DeleteLikeAsync(like.ArticleId, _userId);

            Assert.That(result, Is.True);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(like), Times.Once);
        }

        [Test]
        public async Task DeleteLikeAsync_ShouldReturnFalse_WhenLikeDoesNotExist()
        {
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(_genreId)).ReturnsAsync(new List<Like>());

            var result = await _likeService.DeleteLikeAsync(Guid.NewGuid(), _userId);

            Assert.That(result, Is.False);
            _likeRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Like>()), Times.Never);
        }

        [Test]
        public async Task GetLikesByArticleAsync_ShouldReturnLikes_WhenLikesExist()
        {
            var like = new Like(_userId, Guid.NewGuid());
            var likes = new List<Like> { like };
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like.ArticleId)).ReturnsAsync(likes);

            var result = await _likeService.GetLikesForArticleAsync(like.ArticleId);

            Assert.That(likes, Is.EqualTo(result));
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(like.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetLikesByArticleAsync_ShouldThrowInvalidOperationException_WhenLikesNotFound()
        {
            var articleId = Guid.NewGuid();
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(articleId)).ReturnsAsync(new List<Like>());

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _likeService.GetLikesForArticleAsync(articleId));
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(articleId), Times.Once);
        }

        [Test]
        public async Task GetLikeById_ShouldReturnLike_WhenLikeExist()
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
    }
}
