using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.LikeServices;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BlogWebApp.Tests.LikeTests
{
    [TestFixture]
    public class LikeRepositoryTests
    {
        private LikeRepository _likeRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new BlogWebAppDbContext(options);
            _likeRepository = new LikeRepository(_db);
        }

        [Test]
        public async Task AddLikeAsync_ShouldAddLike_WhenValid()
        {
            var like = new Like("user1", Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            var result = await _db.Likes.FindAsync(like.LikeId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetLikeByIdAsync_ShouldReturnLike_WhenLikeExist()
        {
            var like = new Like("1", Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            var result = await _likeRepository.GetByIdAsync(like.LikeId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetLikesByArticleIdAsync_ShouldReturnLikes_WhenLikesExist()
        {
            var like1 = new Like("user1", Guid.NewGuid());
            var like2 = new Like("user2", like1.ArticleId);
            var like3 = new Like("user3", Guid.NewGuid());

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _likeRepository.AddAsync(like3);

            var result = await _likeRepository.GetByArticleIdAsync(like1.ArticleId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetLikesByArticleIdAsync_ShouldReturnEmpty_WhenArticleHasNoLikes()
        {
            var result = await _likeRepository.GetByArticleIdAsync(It.IsAny<Guid>());

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetLikesByUserIdAsync_ShouldReturnLikes_WhenLikesExist()
        {
            var like1 = new Like("user1", Guid.NewGuid());
            var like2 = new Like("user1", Guid.NewGuid());
            var like3 = new Like("user3", Guid.NewGuid());

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _likeRepository.AddAsync(like3);

            var result = await _likeRepository.GetByUserIdAsync(like1.UserId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetLikesByUserIdAsync_ShouldReturnEmpty_WhenUserHasNoLikes()
        {
            var result = await _likeRepository.GetByUserIdAsync(It.IsAny<string>());

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Any(), Is.False);
        }

        [Test]
        public async Task ExistLikeAsync_ShouldReturnTrue_WhenLikeExist()
        {
            var like = new Like("user1", Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            var result = await _likeRepository.GetByArticleIdAndUserIdAsync(like.ArticleId, like.UserId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task DeleteLikesByArticleIdAsync_ShouldDeleteLikes_WhenExist()
        {
            var like1 = new Like("user1", Guid.NewGuid());
            var like2 = new Like("user2", like1.ArticleId);

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _db.SaveChangesAsync();

            await _likeRepository.DeleteRangeAsync([like1, like2]);

            var result = await _likeRepository.GetByArticleIdAsync(like1.ArticleId);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteLikesByUserIdAsync_ShouldDeleteLikes_WhenExist()
        {
            var like1 = new Like("user1", Guid.NewGuid());
            var like2 = new Like("user1", Guid.NewGuid());

            await _likeRepository.AddAsync(like1);
            await _likeRepository.AddAsync(like2);
            await _db.SaveChangesAsync();

            await _likeRepository.DeleteRangeAsync([like1, like2]);

            var result = await _likeRepository.GetByUserIdAsync(like1.UserId);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteLikeAsync_ShouldDeleteLike_WhenLikeExist()
        {
            var like = new Like("user1", Guid.NewGuid());

            await _likeRepository.AddAsync(like);
            await _likeRepository.DeleteAsync(like);
            var result = await _likeRepository.GetByArticleIdAndUserIdAsync(like.ArticleId, like.UserId);

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
