using BlogWebApp.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BlogWebApp.Tests
{
    public class ArticleServiceTests
    {
        [Test]
        public void CreateArticle_ShouldReturnPost_WhenValid()
        {
            // Arrange
            var date = DateOnly.FromDateTime(DateTime.Now);
            // Act
            var article = new Article(1, "testtitle", "image", "testcontent", 1, 1, date, null);

            // Assert
            Assert.IsNotNull(article);
            Assert.IsTrue(article.ArticleId == 1, article.Title = "testtitle", article.Image == "image", article.Content == "testcontent", article.Likes == 1, article.GenreId == 1, article.CreatedAt == date, article.UpdatedAt == null);

        }
    }
}
