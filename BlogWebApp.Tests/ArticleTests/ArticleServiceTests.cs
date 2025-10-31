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

namespace BlogWebApp.Tests.ArticleTests
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private ArticleService _articleServiceMock;

        [SetUp]
        public void Setup()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _articleServiceMock = new ArticleService(_articleRepositoryMock.Object);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldReturnArticle_WhenValid()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Article>())).ReturnsAsync(article);

            var result = await _articleServiceMock.CreateArticleAsync(article);

            Assert.That(result, Is.EqualTo(article));
            _articleRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Article>()), Times.Once);
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenTitleEmpty()
        {
            var article = new Article(Guid.NewGuid(), null, "testimage", "testcontent", Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleServiceMock.CreateArticleAsync(article));
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenContentEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "testimage", null, Guid.NewGuid());

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleServiceMock.CreateArticleAsync(article));
        }

        [Test]
        public async Task CreateArticleAsync_ShouldThrowArgumentException_WhenGenreGuidEmpty()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "testimage", "testcontent", Guid.Empty);

            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _articleServiceMock.CreateArticleAsync(article));
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenArticleExist()
        {
            var existingArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            await _articleServiceMock.CreateArticleAsync(existingArticle);
            var result = await _articleServiceMock.GetArticleByIdAsync(existingArticle.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(existingArticle.ArticleId));
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(existingArticle.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldCallRepositoryDelete_WhenArticleExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());           
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(article.ArticleId)).ReturnsAsync(article);

            await _articleServiceMock.DeleteArticleAsync(article.ArticleId);

            _articleRepositoryMock.Verify(r => r.DeleteAsync(article), Times.Once);
            _articleRepositoryMock.Verify(r => r.GetByIdAsync(article.ArticleId), Times.Once);
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldThrowInvalidOperationException_WhenArticleNotFound()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Article?)null);

            _articleRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Article>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleServiceMock.DeleteArticleAsync(article.ArticleId));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenArticleExist()
        {
            var existingArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.NewGuid());
            var updatedArticle = new Article(existingArticle.ArticleId, "newtitle", "newimage", "newcontent", Guid.NewGuid());
            _articleRepositoryMock.Setup(r => r.GetByIdAsync(existingArticle.ArticleId)).ReturnsAsync(existingArticle);

            await _articleServiceMock.UpdateArticleAsync(updatedArticle);

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

            _articleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Article>()), Times.Never);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _articleServiceMock.UpdateArticleAsync(article));
        }
    }
}
