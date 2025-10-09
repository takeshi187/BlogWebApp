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

namespace BlogWebApp.Tests
{
    [TestFixture]
    public class ArticleServiceTests
    {
        private DateOnly _date;

        [SetUp]
        public void Setup()
        {
            _date = DateOnly.FromDateTime(DateTime.Now);
        }
        [Test]
        public void CreateArticle_ShouldReturnArticle_WhenValid()
        {
            // Arrange

            // Act
            var article = new Article("testtitle", "image", "testcontent", 1, 1, _date, null);
            article.ArticleId = 1;

            // Assert
            Assert.That(article, Is.Not.Null);
            Assert.That(article.ArticleId, Is.EqualTo(1));
            Assert.That("testtitle", Is.EqualTo(article.Title));
            Assert.That("image", Is.EqualTo(article.Image));
            Assert.That("testcontent", Is.EqualTo(article.Content));
            Assert.That(1, Is.EqualTo(article.Likes));           
            Assert.That(1, Is.EqualTo(article.GenreId));        
            Assert.That(_date, Is.EqualTo(article.CreatedAt));  
            Assert.That(article.UpdatedAt, Is.Null);           

        }
    }
}
