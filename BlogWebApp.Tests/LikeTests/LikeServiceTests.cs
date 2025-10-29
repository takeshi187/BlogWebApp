using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.Services.UserServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests.LikeTests
{
    [TestFixture]
    public class LikeServiceTests
    {
        private Mock<ILikeRepository> _likeRepositoryMock;
        private Mock<IArticleService> _articleServiceMock;
        private Mock<IUserService> _userServiceMock;
        private LikeService _likeService;
        private Guid _genreId;
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            _likeRepositoryMock = new Mock<ILikeRepository>();
            _articleServiceMock = new Mock<IArticleService>();
            _userServiceMock = new Mock<IUserService>();
            _likeService = new LikeService(_likeRepositoryMock.Object, _userServiceMock.Object, _articleServiceMock.Object);
            _genreId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
            _userId = "b1b2c3d4-e5f6-7890-1234-567890abcdef";
        }

        [Test]
        public async Task AddLikeAsync_ShouldReturnTrue_WhenValid()
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
        public async Task DeleteLikeAsync_ShouldReturnTrue_WhenLikeExist()
        {
            var like = new Like(_userId, Guid.NewGuid());
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like.ArticleId)).ReturnsAsync(new List<Like> { like });

            var result = await _likeService.DeleteLikeAsync(like.ArticleId, _userId);

            Assert.That(result, Is.True);
            _likeRepositoryMock.Verify(r=>r.DeleteAsync(like), Times.Once);
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
        public async Task GetLikesForArticleAsync_ShouldReturnLikes_WhenExist()
        {
            var like = new Like(_userId, Guid.NewGuid());
            var likes = new List<Like> { like };
            _likeRepositoryMock.Setup(r => r.GetByArticleIdAsync(like.ArticleId)).ReturnsAsync(likes);

            var result = await _likeService.GetLikesForArticleAsync(like.ArticleId);

            Assert.That(likes, Is.EqualTo(result));
            _likeRepositoryMock.Verify(r => r.GetByArticleIdAsync(like.ArticleId), Times.Once);
        }
    }
}
