using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
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
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Article>())).ReturnsAsync(article);

            var result = await _articleService.CreateArticleAsync(article);

            Assert.That(result, Is.EqualTo(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenTitleEmpty()
        {
            var article = new Article(Guid.NewGuid(), "", "testimage", "testcontent", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.CreateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "testimage", "", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.CreateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenGenreGuidEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "testimage", "testcontent", Guid.Empty);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.CreateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenArticleEmpty()
        {
            Article article = null;

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.CreateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _articleService.CreateArticleAsync(article);
            var result = await _articleService.GetArticleByIdAsync(article.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(article.ArticleId));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldThrowInvalidOperation_WhenArticleNotFound()
        {
            var articleId = Guid.NewGuid();
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(articleId)).ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.GetArticleByIdAsync(articleId));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(articleId), Times.Once);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldThrowArgumentException_WhenArticleIdEmpty()
        {
            var articleId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.GetArticleByIdAsync(articleId));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(articleId), Times.Never);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _articleService.DeleteArticleAsync(article.ArticleId);

            _articleRepositoryMock.Verify(r => r.DeleteAsync(article), Times.Once);
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.DeleteArticleAsync(article.ArticleId));
            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldArgumentException_WhenArticleIdEmpty()
        {
            var articleId = Guid.Empty;

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.DeleteArticleAsync(articleId));
            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var existingArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            var updatedArticle = new Article(existingArticle.ArticleId, "newtitle", "newimage", "newcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            var result = await _articleService.UpdateArticleAsync(updatedArticle);

            Assert.That(result, Is.EqualTo(existingArticle));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(existingArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Article>(a =>
                a.ArticleId == existingArticle.ArticleId &&
                a.Title == "newtitle" &&
                a.Image == "newimage" &&
                a.Content == "newcontent" &&
                a.GenreId == updatedArticle.GenreId &&
                a.UpdatedAt.HasValue
                )), Times.Once);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync((Article?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.UpdateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowArgumentException_WhenArticleEmpty()
        {
            Article article = null;

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.UpdateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowArgumentException_WhenTitleEmpty()
        {
            var article = new Article(Guid.NewGuid(), "", "image", "testcontent", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.UpdateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.UpdateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowArgumentException_WhenGenreIdEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.Empty);

            Assert.ThrowsAsync<ArgumentException>(async () => await _articleService.UpdateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
        }
    }
}
