using BlogWebApp.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using NUnit.Framework.Legacy;
using BlogWebApp.Services.ArticleServices;
using NUnit.Framework.Internal;
using Microsoft.Extensions.Logging;


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
            var article = new Article(Guid.NewGuid(), null, "testimage", "testcontent", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleService.CreateArticleAsync(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Never);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "testimage", null, Guid.NewGuid());

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
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var existingArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            await _articleService.CreateArticleAsync(existingArticle);
            var result = await _articleService.GetArticleByIdAsync(existingArticle.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(existingArticle.ArticleId));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(existingArticle.ArticleId), Times.Once);
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

            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.DeleteArticleAsync(article.ArticleId));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var existingArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            var updatedArticle = new Article(existingArticle.ArticleId, "newtitle", "newimage", "newcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            var result = await _articleService.UpdateArticleAsync(updatedArticle);

            _articleRepositoryMock.Verify(r => r.GetByIdAsync(existingArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Article>(a =>
                a.ArticleId == existingArticle.ArticleId &&
                a.Title == "newtitle" &&
                a.Image == "newimage" &&
                a.Content == "newcontent" &&
                a.GenreId == updatedArticle.GenreId &&
                a.UpdatedAt.HasValue
                )), Times.Once);
            Assert.That(result, Is.EqualTo(existingArticle));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync((Article?)null);

            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.UpdateArticleAsync(article));
        }
    }
}
