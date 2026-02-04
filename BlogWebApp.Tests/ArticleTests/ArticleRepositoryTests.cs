using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.GenreServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Tests.ArticleTests
{
    [TestFixture]
    public class ArticleRepositoryTests
    {
        private ArticleRepository _articleRepository;
        private GenreRepository _genreRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new BlogWebAppDbContext(options);
            _articleRepository = new ArticleRepository(_db);
            _genreRepository = new GenreRepository(_db);
        }

        [Test]
        public async Task AddArticleAsync_ShouldAddArticle_WhenValid()
        {
            var genre = new Genre("testgenre");
            await _genreRepository.AddAsync(genre);

            var article = new Article("testtitle", "testimage", "testcontent", genre.GenreId);
            await _articleRepository.AddAsync(article);

            var articles = await _articleRepository.GetAllAsync();

            Assert.That(articles.Count, Is.EqualTo(1));
            Assert.That(articles.First().Title, Is.EqualTo("testtitle"));
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var genre = new Genre("testgenre");
            await _genreRepository.AddAsync(genre);

            var article = new Article("testtitle", "testimage", "testcontent", genre.GenreId);
            await _articleRepository.AddAsync(article);

            var result = await _articleRepository.GetByIdAsync(article.ArticleId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnArticles_WhenArticlesExist()
        {
            var genre = new Genre("testgenre");
            await _genreRepository.AddAsync(genre);

            var article1 = new Article("testtitle", "image", "testcontent", genre.GenreId);
            var article2 = new Article("testtitle", "image", "testcontent", article1.GenreId);
            var article3 = new Article("testtitle", "image", "testcontent", article1.GenreId);

            await _articleRepository.AddAsync(article1);
            await _articleRepository.AddAsync(article2);
            await _articleRepository.AddAsync(article3);

            var result = await _articleRepository.GetAllAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var genre = new Genre("testgenre");
            await _genreRepository.AddAsync(genre);

            var article = new Article("old", null, "old content", genre.GenreId);
            await _articleRepository.AddAsync(article);
            
            article.Update("new", "img", "new content", genre.GenreId);
            await _articleRepository.UpdateAsync(article);

            var updated = await _articleRepository.GetByIdAsync(article.ArticleId);

            Assert.That(updated!.Title, Is.EqualTo("new"));
            Assert.That(updated.Content, Is.EqualTo("new content"));
            Assert.That(updated.UpdatedAt, Is.Not.Null);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenArticleExist()
        {
            var genre = new Genre("testgenre");
            await _genreRepository.AddAsync(genre);

            var article = new Article("testtitle", "image", "testcontent", genre.GenreId);
            await _articleRepository.AddAsync(article);
            await _articleRepository.DeleteAsync(article);

            var result = await _articleRepository.GetByIdAsync(article.ArticleId);

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
