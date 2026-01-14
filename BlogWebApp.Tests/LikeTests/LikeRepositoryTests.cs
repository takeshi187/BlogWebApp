using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.LikeServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Tests.LikeTests
{
    [TestFixture]
    public class LikeRepositoryTests
    {
        private LikeRepository _likeRepository;
        private BlogWebAppDbContext _db;
        private Guid _articleId;
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase("TestDatabase").Options;

            _db = new BlogWebAppDbContext(options);
            _likeRepository = new LikeRepository(_db);
            _articleId = Guid.Parse("a1b2c3d4-e5f6-7890-1234-567890abcdef");
            _userId = "b1b2c3d4-e5f6-7890-1234-567890abcdef";
        }

        [Test]
        public async Task AddLikeAsync_ShouldAddLike_WhenValid()
        {
            var like = new Like(Guid.NewGuid(), _userId, Guid.NewGuid());

            var result = await _likeRepository.AddAsync(like);

            Assert.That(result, Is.Not.Null);
            Assert.That(like.UserId, Is.EqualTo(result.UserId));
            Assert.That(like.ArticleId, Is.EqualTo(result.ArticleId));
            Assert.That(like.LikeId, Is.EqualTo(result.LikeId));
            Assert.That(like.CreatedAt.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public async Task GetLikeByIdAsync_ShouldReturnLike_WhenLikeExist()
        {
            var like = new Like(Guid.NewGuid(), _userId, Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            var result = await _likeRepository.GetByIdAsync(like.LikeId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(like));
        }

        [Test]
        public async Task GetLikesByArticleIdAsync_ShouldReturnLikes_WhenLikesExist()
        {
            var like1 = new Like(Guid.NewGuid(), "user1", _articleId);
            var like2 = new Like(Guid.NewGuid(), "user2", _articleId);
            var like3 = new Like(Guid.NewGuid(), "user3", Guid.NewGuid());

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _likeRepository.AddAsync(like3);

            var result = await _likeRepository.GetByArticleIdAsync(_articleId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetLikesByArticleIdAsync_ShouldReturnEmpty_WhenArticleHasNoLikes()
        {
            var result = await _likeRepository.GetByArticleIdAsync(Guid.Parse("a2b2c3d4-e5f6-7890-1234-567890abcdef"));

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetLikesByUserIdAsync_ShouldReturnLikes_WhenLikesExist()
        {
            _db.RemoveRange(_db.Likes);
            var like1 = new Like(Guid.NewGuid(), _userId, _articleId);
            var like2 = new Like(Guid.NewGuid(), _userId, _articleId);
            var like3 = new Like(Guid.NewGuid(), "user3", Guid.NewGuid());

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _likeRepository.AddAsync(like3);

            var result = await _likeRepository.GetByUserIdAsync(_userId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetLikesByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoLikes()
        {
            var result = await _likeRepository.GetByUserIdAsync("non-existing-user");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public async Task ExistLikeAsync_ShouldReturnTrue_WhenLikeExist()
        {
            var like = new Like(Guid.NewGuid(), _userId, Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            var result = await _likeRepository.ExistAsync(like.ArticleId, _userId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldDeleteLikesForArticle_WhenExist()
        {
            var articleId = Guid.NewGuid();

            var like1 = new Like(Guid.NewGuid(), "user1", articleId);
            var like2 = new Like(Guid.NewGuid(), "user2", articleId);

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _db.SaveChangesAsync();

            await _likeRepository.DeleteRangeAsync([like1, like2]);

            var result = await _likeRepository.GetByArticleIdAsync(articleId);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteLikeAsync_ShouldDeleteLike_WhenLikeExist()
        {
            var like = new Like(Guid.NewGuid(), _userId, Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            await _likeRepository.DeleteAsync(like);
            var result = await _likeRepository.ExistAsync(like.ArticleId, _userId);

            Assert.That(result, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
