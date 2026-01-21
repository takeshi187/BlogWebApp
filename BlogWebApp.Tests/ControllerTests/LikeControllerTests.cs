using BlogWebApp.Controllers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.LikeServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class LikeControllerTests
    {
        private Mock<ILikeService> _likeServiceMock;
        private LikeController _controller;
        private const string UserId = "user1";

        [SetUp]
        public void SetUp()
        {
            _likeServiceMock = new Mock<ILikeService>();
            _controller = new LikeController(null!, _likeServiceMock.Object);
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            }, "TestAuth"));

            var httpContext = new DefaultHttpContext
            {
                User = user
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            _controller.TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>());
        }

        [Test]
        public async Task ToggleLike_ShouldToggleLike_WhenValid()
        {
            var articleId = Guid.NewGuid();

            _likeServiceMock
                .Setup(s => s.ToggleLikeAsync(articleId, UserId));

            var result = await _controller.ToggleLike(articleId);

            _likeServiceMock.Verify(
                s => s.ToggleLikeAsync(articleId, UserId),
                Times.Once
            );

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Blog"));
        }

        [TearDown]
        public void TearDown()
        {
            if (_controller != null)
            {
                _controller.Dispose();
            }
        }
    }
}
