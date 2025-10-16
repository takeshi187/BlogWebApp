using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests
{
    [TestFixture]
    public class ArticleRepositoryTests
    {
        private ArticleRepository _articleRepository;
        private BlogWebAppDbContext _db;
        private DateOnly _date;

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
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.ArticleId = 1;

            var result = await _articleRepository.AddAsync(newArticle);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("testtitle"));
            Assert.That(result.Image, Is.EqualTo("image"));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
            Assert.That(result.Likes, Is.EqualTo(1));
            Assert.That(result.GenreId, Is.EqualTo(1));
            Assert.That(result.CreatedAt, Is.EqualTo(_date));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenFound()
        {
            var existingArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            existingArticle.ArticleId = 1;

            var result = await _articleRepository.GetByIdAsync(existingArticle.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(existingArticle.ArticleId));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenValid()
        {
            var existingArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            existingArticle.ArticleId = 1;
            var newArticle = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            newArticle.Title = "newtitle";

            var result = await _articleRepository.UpdateAsync(newArticle);

            Assert.That(newArticle.Title, Is.EqualTo("newtitle"));
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenExist()
        {
            var article = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            var createdArticle = await _articleRepository.AddAsync(article);

            await _articleRepository.DeleteAsync(createdArticle.ArticleId);
            var result = await _articleRepository.GetByIdAsync(createdArticle.ArticleId);

            Assert.That(result, Is.Null);           
        }

        [TearDown]
        public void TearDown()
        {
            if(_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
