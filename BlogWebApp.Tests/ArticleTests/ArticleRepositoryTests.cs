using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Tests.ArticleTests
{
    [TestFixture]
    public class ArticleRepositoryTests
    {
        private ArticleRepository _articleRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase("TestDatabase").Options;

            _db = new BlogWebAppDbContext(options);
            _articleRepository = new ArticleRepository(_db);
        }

        [Test]
        public async Task AddArticleAsync_ShouldAddArticle_WhenValid()
        {
            var article = new Article("testtitle", "image", "testcontent", Guid.NewGuid());

            var result = await _articleRepository.AddAsync(article);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("testtitle"));
            Assert.That(result.Image, Is.EqualTo("image"));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
            Assert.That(result.GenreId, Is.EqualTo(article.GenreId));
            Assert.That(result.CreatedAt.Date, Is.EqualTo(DateTime.UtcNow.Date));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());

            await _articleRepository.AddAsync(article);
            var result = await _articleRepository.GetByIdAsync(article.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(article.ArticleId));
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            var article1 = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            var article2 = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            var article3 = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());

            _db.Articles.RemoveRange(_db.Articles);
            await _db.SaveChangesAsync();
            await _articleRepository.AddAsync(article1);
            await _articleRepository.AddAsync(article2);
            await _articleRepository.AddAsync(article3);

            var result = await _articleRepository.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());

            await _articleRepository.AddAsync(article);
            article.Title = "newtitle";
            article.UpdatedAt = DateTime.UtcNow;

            var result = await _articleRepository.UpdateAsync(article);

            Assert.That(result, Is.True);
            Assert.That(article.Title, Is.EqualTo("newtitle"));
            Assert.That(article.UpdatedAt?.Date, Is.EqualTo(DateTime.UtcNow.Date));
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());

            await _articleRepository.AddAsync(article);
            var result = await _articleRepository.DeleteAsync(article);

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
