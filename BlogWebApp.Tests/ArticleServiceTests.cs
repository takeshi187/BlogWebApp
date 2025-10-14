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
            // Arrange
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            _articleRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Article>())).ReturnsAsync(newArticle);

            // Act
            var result = await _articleService.CreateArticleAsync("testtitle", "image", "testcontent", 1);

            // Assert
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
            // Arrange
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(newArticle.ArticleId)).ReturnsAsync(newArticle);

            // Act
            var result = await _articleService.GetArticleByIdAsync(newArticle.ArticleId);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(1));
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(newArticle.ArticleId), Times.Once);
        }

        [Test]
        public async Task GetAllArticlesAsync_ShouldReturnListOfArticles_WhenExist()
        {
            // Arrange
            var Articles = new List<Article>()
            {
                new Article("testtitle", "image", "testcontent", 1, 1, _date, null),
                new Article("testtitle", "image", "testcontent", 1, 1, _date, null),
            };

            _articleRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(Articles);

            // Act
            var result = await _articleService.GetAllArticlesAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(Articles.Count));
            _articleRepositoryMock.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldCallRepositoryDelete_WhenArticleExist()
        {
            // Arrange
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(newArticle.ArticleId)).ReturnsAsync(newArticle);

            // Act
            await _articleService.DeleteArticleAsync(newArticle.ArticleId);

            // Assert
            _articleRepositoryMock.Verify(repo => repo.DeleteAsync(newArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(newArticle.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldReturnFalse_WhenArticleNotFound()
        {
            // Arrange
            var articleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Article?)null);

            // Act
            var result = await _articleService.DeleteArticleAsync(articleId);

            // Assert
            Assert.That(result, Is.False);
            _articleRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(articleId), Times.Once);
        }
        // to do переделать тест, добавить тест на отсутствие обновляемого article
        [Test]
        public async Task UpdateArticleAsync_ShouldReturnUpdateArticle_WhenArticleExist()
        {
            // Arrange
            var oldArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            var newArticle = new Article("newtitle", "newimage", "newcontent", 1, 2, _date, DateOnly.FromDateTime(DateTime.Now));
            oldArticle.ArticleId = 1;
            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(oldArticle.ArticleId)).ReturnsAsync(oldArticle);

            // Act
            await _articleService.UpdateArticleAsync(1, "newtitle", "newimage", "newcontent", 2);

            // Assert
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(oldArticle.ArticleId), Times.Once);
            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Article>(a =>
                a.ArticleId == newArticle.ArticleId &&
                a.Title == "newtitle" &&
                a.Image == "newimage" &&
                a.Content == "newcontent" &&
                a.GenreId == 2 &&
                a.UpdatedAt.HasValue
                )), Times.Once);
        }
    }
}
