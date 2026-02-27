using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;


namespace BlogWebApp.Tests.ArticleTests
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<ILogger<ArticleService>> _loggerMock;
        private ArticleService _articleService;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _loggerMock = new Mock<ILogger<ArticleService>>();
            _articleService = new ArticleService(_articleRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldCreateArticle_WhenValid()
        {
            _articleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Article>()));

            await _articleService.CreateArticleAsync("testtitle", "testimage", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Verify(r =>
                r.AddAsync(It.Is<Article>(a =>
                    a.Title == "testtitle" &&
                    a.Image == "testimage" &&
                    a.Content == "testcontent"
                )),
                Times.Once);
        }

        [Test]
        public void CreateArticleAsync_ShouldThrowDbUpdateException_WhenDbFails()
        {
            _articleRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Article>()))
                .ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _articleService.CreateArticleAsync("testtitle", "testimage", "testcontent", Guid.NewGuid()));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var article = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());

            _articleRepositoryMock
                .Setup(r => r.GetByIdAsync(article.ArticleId))
                .ReturnsAsync(article);

            var result = await _articleService.GetArticleByIdAsync(article.ArticleId);

            Assert.That(result, Is.EqualTo(article));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnNull_WhenArticleNotFound()
        {
            var articleId = Guid.NewGuid();
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId)).ReturnsAsync((Article?)null);

            var result = await _articleService.GetArticleByIdAsync(articleId);

            Assert.That(result, Is.Null);
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(articleId), Times.Once);
        }

        [Test]
        public void GetArticleByIdAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.GetArticleByIdAsync(Guid.Empty));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnAllArticles_WhenArticlesExist()
        {
            var article1 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article2 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article3 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var articles = new List<Article> { article1, article2, article3 };
            _articleRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(articles);

            var result = await _articleService.GetAllArticlesAsync();

            Assert.That(articles, Is.EqualTo(articles));
            _articleRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnEmptyList_WhenNoArticlesExist()
        {
            _articleRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Article>());

            var result = await _articleService.GetAllArticlesAsync();

            Assert.That(result, Is.Empty);
            _articleRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldThrowDbUpdateException_WhenErrorOccurs()
        {
            _articleRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            var ex = Assert.ThrowsAsync<Exception>(async () => await _articleService.GetAllArticlesAsync());
            Assert.That(ex.Message, Is.EqualTo("Database error"));
            _articleRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenArticleExist()
        {
            var article = new Article("testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _articleService.DeleteArticleAsync(article.ArticleId);

            _articleRepositoryMock.Verify(r => r.DeleteAsync(article), Times.Once);
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldReturnFalse_WhenArticleNotFound()
        {
            var article = new Article("testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Article?)null);

            var result = await _articleService.DeleteArticleAsync(article.ArticleId);

            Assert.That(result, Is.False);
            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldArgumentException_WhenArticleIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.DeleteArticleAsync(Guid.Empty));
            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldThrowDbUpdateException_WhenDatabaseError()
        {
            var article = new Article("Title", "Image", "Content", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _articleRepositoryMock.Setup(r => r.DeleteAsync(article)).ThrowsAsync(new DbUpdateException());

            var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _articleService.DeleteArticleAsync(article.ArticleId));
            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var article = new Article("old", "old", "old", Guid.NewGuid());

            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _articleService.UpdateArticleAsync(article.ArticleId, "newtitle", "newimage", "newcontent", Guid.NewGuid());

            _articleRepositoryMock.Verify(r =>
                r.UpdateAsync(It.Is<Article>(a =>
                    a.Title == "newtitle" &&
                    a.Image == "newimage" &&
                    a.Content == "newcontent"
                )),
                Times.Once);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldReturnFalse_WhenArticleNotFound()
        {
            var article = new Article("testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Article?)null);

            var result = await _articleService.UpdateArticleAsync(article.ArticleId, "newtitle", "newimage", "newcontent", Guid.NewGuid());

            Assert.That(result, Is.False);
            _articleRepositoryMock.Verify(r =>
                r.UpdateAsync(It.Is<Article>(a =>
                    a.Title == "newtitle" &&
                    a.Image == "newimage" &&
                    a.Content == "newcontent"
                )),
                Times.Never);

        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.UpdateArticleAsync(Guid.Empty, "NewTitle", "NewImage", "NewContent", Guid.NewGuid()));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowDbUpdateException_WhenInvalid()
        {
            var existingArticle = new Article("OldTitle", "OldImage", "OldContent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);
            _articleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Article>())).ThrowsAsync(new DbUpdateException());

            var ex = Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _articleService.UpdateArticleAsync(existingArticle.ArticleId, "NewTitle", "NewImage", "NewContent", Guid.NewGuid()));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Once);
        }
    }
}
