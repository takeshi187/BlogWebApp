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
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase("TestDatabase").Options;

            _db = new BlogWebAppDbContext(options);
            _commentRepository = new CommentRepository(_db);
        }

        [Test]
        public async Task AddCommentAsync_ShouldAddComment_WhenValid()
        {
            var comment = new Comment(Guid.NewGuid(), "testcontent", "1", Guid.NewGuid());

            var result = await _commentRepository.AddAsync(comment);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.EqualTo(comment.Content));
            Assert.That(result.UserId, Is.EqualTo(comment.UserId));
            Assert.That(result.ArticleId, Is.EqualTo(comment.ArticleId));
        }

        [Test]
        public async Task GetCommentByIdAsync_ShouldReturnComment_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment);
            var result = await _commentRepository.GetByIdAsync(comment.CommentId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.CommentId, Is.EqualTo(result.CommentId));
        }

        [Test]
        public async Task GetCommentsByArticleIdAsync_ShouldReturnComments_WhenCommentsExist()
        {
            var comment1 = new Comment(Guid.NewGuid(), "testcontent", "1", Guid.NewGuid());
            var comment2 = new Comment(Guid.NewGuid(), "testcontent", "2", comment1.ArticleId);
            var comment3 = new Comment(Guid.NewGuid(), "testcontent", "3", Guid.NewGuid());

            await _commentRepository.AddAsync(comment1);
            await _commentRepository.AddAsync(comment2);
            await _commentRepository.AddAsync(comment3);

            var result = await _commentRepository.GetByArticleIdAsync(comment1.ArticleId);

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateCommentAsync_ShouldUpdateComment_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment);
            comment.Content = "newcontent";
            comment.UpdatedAt = DateTime.UtcNow;

            var result = await _commentRepository.UpdateAsync(comment);

            Assert.That(result, Is.True);
            Assert.That(comment.Content, Is.EqualTo("newcontent"));
            Assert.That(comment.UpdatedAt?.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public async Task DeleteCommentsByArticleIdAsync_ShouldDeleteCommentsForArticle_WhenExist()
        {
            var articleId = Guid.NewGuid();

            var comment1 = new Comment(Guid.NewGuid(), "content1", "user1", articleId);
            var comment2 = new Comment(Guid.NewGuid(), "content2", "user2", articleId);

            await _db.Comments.AddAsync(comment1);
            await _db.Comments.AddAsync(comment2);
            await _db.SaveChangesAsync();

            await _commentRepository.DeleteRangeAsync([comment1, comment2]);
            var result = await _commentRepository.GetByArticleIdAsync(articleId);

            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteCommentAsync_ShouldDeleteComment_WhenCommentExist()
        {
            var comment = new Comment(Guid.NewGuid(), "testcontent", "1", Guid.NewGuid());

            await _commentRepository.AddAsync(comment);
            var result = await _commentRepository.DeleteAsync(comment);

            Assert.That(result, Is.True);
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
