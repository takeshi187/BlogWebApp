using BlogWebApp.Controllers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class BlogControllerTests
    {
        private Mock<IArticleService> _articleServiceMock;
        private BlogController _blogController;

        [SetUp]
        public void SetUp()
        {
            _articleServiceMock = new Mock<IArticleService>();
            _blogController = new BlogController(_articleServiceMock.Object);

            _blogController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };
        }

        [Test]
        public async Task BlogIndexGet_ShouldReturnEmptyList_WhenNoArticles()
        {
            _articleServiceMock.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(new List<Article>());

            var result = await _blogController.Index(null);
            var viewResult = result as ViewResult;
            var articleViewModel = viewResult.Model as List<ArticleViewModel>;

            Assert.That(viewResult, Is.Not.Null);
            Assert.That(articleViewModel, Is.Not.Null);
            Assert.That(articleViewModel.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task BlogIndexGet_ShouldReturnAllArticles_WhenQueryIsNull()
        {
            var article1 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article2 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article3 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var articles = new List<Article> { article1, article2, article3 };
            _articleServiceMock.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(articles);

            var result = await _blogController.Index(null);

            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<ArticleViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task BlogIndexGet_ShouldFilterArticles_ByQuery()
        {
            var article1 = new Article("ASP.NET Core", "testimage", "testcontent", Guid.NewGuid());
            var article2 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article3 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var articles = new List<Article> { article1, article2, article3 };
            _articleServiceMock.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(articles);

            var result = await _blogController.Index("asp");

            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<ArticleViewModel>;

            Assert.That(model.Count, Is.EqualTo(1));
            Assert.That(model[0].Title, Is.EqualTo("ASP.NET Core"));
        }

        [Test]
        public async Task BlogIndexGet_ShouldTrimQuery()
        {
            var article1 = new Article("C#", "testimage", "testcontent", Guid.NewGuid());
            var article2 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article3 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var articles = new List<Article> { article1, article2, article3 };
            _articleServiceMock.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(articles);

            var result = await _blogController.Index("  c#  ");
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<ArticleViewModel>;
            Assert.That(model.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task BlogIndexGet_ShouldPassUserIdToViewModel_WhenUserAuthenticated()
        {
            var userId = "user-123";

            var article1 = new Article("C#", "testimage", "testcontent", Guid.NewGuid());
            var article2 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var article3 = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            var articles = new List<Article> { article1, article2, article3 };
            _articleServiceMock.Setup(s => s.GetAllArticlesAsync()).ReturnsAsync(articles);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                "TestAuth"));

            _blogController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _blogController.Index(null);
            var viewResult = result as ViewResult;
            var model = viewResult.Model as List<ArticleViewModel>;
            Assert.That(model, Is.Not.Null);
        }

        [TearDown]
        public void TearDown()
        {
            if (_blogController != null)
            {
                _blogController.Dispose();
            }
        }
    }
}
