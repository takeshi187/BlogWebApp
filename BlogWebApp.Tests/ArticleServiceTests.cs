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
using BlogWebApp.Services;

namespace BlogWebApp.Tests
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private ArticleService _articleService;
        private DateOnly _date;

        [SetUp]
        public void Setup()
        {
            _date = DateOnly.FromDateTime(DateTime.Now);
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _articleService = new ArticleService(_articleRepositoryMock.Object);

        }

        [Test]
        public async Task CreateArticleAsync_ShouldReturnArticle_WhenValid()
        {
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            _articleRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Article>())).ReturnsAsync(newArticle);

            var result = await _articleService.CreateArticleAsync("testtitle", "image", "testcontent", 1);

            Assert.That(result, Is.Not.Null);
            Assert.That("testtitle", Is.EqualTo(result.Title));
            Assert.That("image", Is.EqualTo(result.Image));
            Assert.That("testcontent", Is.EqualTo(result.Content));
            Assert.That(1, Is.EqualTo(result.Likes));
            Assert.That(1, Is.EqualTo(result.GenreId));
            Assert.That(_date, Is.EqualTo(result.CreatedAt));
            Assert.That(result.UpdatedAt, Is.Null);
            _articleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenExist()
        {
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(newArticle.ArticleId)).ReturnsAsync(newArticle);

            var result = await _articleService.GetArticleByIdAsync(newArticle.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(1));
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(newArticle.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnListOfArticles_WhenExist()
        {
            var Articles = new List<Article>()
            {
                new Article("testtitle", "image", "testcontent", 1, 1, _date, null),
                new Article("testtitle", "image", "testcontent", 1, 1, _date, null),
            };

            _articleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(Articles);

            var result = await _articleService.GetAllArticlesAsync();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(Articles.Count));
            _articleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldCallRepositoryDelete_WhenArticleExist()
        {
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(newArticle.ArticleId)).ReturnsAsync(newArticle);

            await _articleService.DeleteArticleAsync(newArticle.ArticleId);

            _articleRepositoryMock.Verify(repo => repo.DeleteAsync(newArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(newArticle.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldReturnFalse_WhenArticleNotFound()
        {
            var articleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article?)null);

            var result = await _articleService.DeleteArticleAsync(articleId);

            Assert.That(result, Is.False);
            _articleRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(articleId), Times.Once);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var existingArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            var updatedArticle = new Article("newtitle", "newimage", "newcontent", 1, 2, _date, DateOnly.FromDateTime(DateTime.Now));
            existingArticle.ArticleId = 1;
            updatedArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            await _articleService.UpdateArticleAsync(updatedArticle);

            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(existingArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Article>(a =>
                a.ArticleId == existingArticle.ArticleId &&
                a.Title == "newtitle" &&
                a.Image == "newimage" &&
                a.Content == "newcontent" &&
                a.GenreId == 2 &&
                a.UpdatedAt.HasValue
                )), Times.Once);
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var article = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            article.ArticleId = 0;

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(article.ArticleId)).ReturnsAsync((Article?)null);

            _articleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Article>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleService.UpdateArticleAsync(article));

        }
    }
}
