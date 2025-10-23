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
        private string _guidString;
        private string _guidStringNew;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase("TestDatabase").Options;

            _db = new BlogWebAppDbContext(options);
            _articleRepository = new ArticleRepository(_db);
            _date = DateOnly.FromDateTime(DateTime.UtcNow);
            _guidString = "a1b2c3d4-e5f6-7890-1234-567890abcdef";
            _guidStringNew = "b1b2c3d4-e5f6-7890-1234-567890abcdef";
        }

        [Test]
        public async Task AddArticleAsync_ShouldAddArticle_WhenValid()
        {
            var newArticle = new Article("testtitle", "image", "testcontent", Guid.Parse(_guidString));

            var result = await _articleRepository.AddAsync(newArticle);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo("testtitle"));
            Assert.That(result.Image, Is.EqualTo("image"));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
            Assert.That(result.GenreId, Is.EqualTo(Guid.Parse(_guidString)));
            Assert.That(result.CreatedAt, Is.EqualTo(_date));
            Assert.That(result.Content, Is.EqualTo("testcontent"));
        }

        [Test]
        public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenFound()
        {
            var newArticle = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.Parse(_guidString));
            
            await _articleRepository.AddAsync(newArticle);
            var result = await _articleRepository.GetByIdAsync(newArticle.ArticleId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ArticleId, Is.EqualTo(newArticle.ArticleId));
        }

        [Test]
        public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenValid()
        {
            var existingArticle = await _articleRepository.GetByIdAsync(Guid.Parse(_guidString));

            if(existingArticle == null)
            {
                existingArticle = new Article(Guid.Parse(_guidString), "testtitle", "image", "testcontent", Guid.Parse(_guidString));
                await _articleRepository.AddAsync(existingArticle);
            }
            
            var newArticle = existingArticle;
            newArticle.Title = "newtitle";

            var result = await _articleRepository.UpdateAsync(newArticle);

            Assert.That(result.Title, Is.EqualTo("newtitle"));
        }

        [Test]
        public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenExist()
        {
            var article = new Article(Guid.NewGuid(), "testtitle", "image", "testcontent", Guid.Parse(_guidString));
            var createdArticle = await _articleRepository.AddAsync(article);

            await _articleRepository.DeleteAsync(createdArticle);
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
