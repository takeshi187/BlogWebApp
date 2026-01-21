using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Tests.CommentTests
{
    [TestFixture]
    public class CommentRepositoryTests
    {
        private CommentRepository _commentRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new BlogWebAppDbContext(options);
            _commentRepository = new CommentRepository(_db);
        }

        [Test]
        public async Task AddCommentAsync_ShouldAddComment_WhenValid()
        {
            var comment = new Comment("testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment);
            var result = await _db.Comments.FindAsync(comment.CommentId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetCommentByIdAsync_ShouldReturnComment_WhenCommentExist()
        {
            var comment = new Comment("testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment);
            var result = await _commentRepository.GetByIdAsync(comment.CommentId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetCommentsByArticleIdAsync_ShouldReturnComments_WhenCommentsExist()
        {
            var comment1 = new Comment("testcontent", "1", Guid.NewGuid());
            var comment2 = new Comment("testcontent", "2", comment1.ArticleId);
            var comment3 = new Comment("testcontent", "3", Guid.NewGuid());

            await _commentRepository.AddAsync(comment1);
            await _commentRepository.AddAsync(comment2);
            await _commentRepository.AddAsync(comment3);

            var result = await _commentRepository.GetByArticleIdAsync(comment1.ArticleId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetCommentsByUserIdAsync_ShouldReturnComments_WhenCommentsExist()
        {
            var comment1 = new Comment("testcontent", "1", Guid.NewGuid());
            var comment2 = new Comment("testcontent", "2", comment1.ArticleId);
            var comment3 = new Comment("testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment1);
            await _commentRepository.AddAsync(comment2);
            await _commentRepository.AddAsync(comment3);

            var result = await _commentRepository.GetByUserIdAsync(comment1.UserId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldDeleteComments_WhenExist()
        {
            var articleId = Guid.NewGuid();

            var comment1 = new Comment("content1", "user1", articleId);
            var comment2 = new Comment("content2", "user2", articleId);

            await _db.Comments.AddAsync(comment1);
            await _db.Comments.AddAsync(comment2);

            await _commentRepository.DeleteRangeAsync([comment1, comment2]);
            var result = await _commentRepository.GetByArticleIdAsync(articleId);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        public async Task DeleteCommentsByUserIdAsync_ShouldDeleteComments_WhenExist()
        {
            var userId = "user1";

            var comment1 = new Comment("content1", userId, Guid.NewGuid());
            var comment2 = new Comment("content2", userId, Guid.NewGuid());

            await _db.Comments.AddAsync(comment1);
            await _db.Comments.AddAsync(comment2);

            await _commentRepository.DeleteRangeAsync([comment1, comment2]);
            var result = await _commentRepository.GetByUserIdAsync(userId);

            Assert.That(result.Count, Is.EqualTo(0));
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
