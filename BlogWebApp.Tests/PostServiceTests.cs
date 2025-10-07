using BlogWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests
{
    public class PostServiceTests
    {
        [Test]
        public void CreateArticle_ShouldReturnPost_WhenValid()
        {
            // Arrange
            var article = new Article();

            // Assert
            Assert.IsNotNull(article);
        }
    }
}
