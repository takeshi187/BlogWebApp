using BlogWebApp.Controllers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.FileStorageServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class ArticleControllerTests
    {
        private Mock<IArticleService> _articleServiceMock;
        private Mock<IGenreService> _genreServiceMock;
        private Mock<ICommentService> _commentServiceMock;
        private Mock<ILikeService> _likeServiceMock;
        private Mock<IImageStorageService> _imageStorageMock;
        private ArticleController _articleController;

        private const string UserId = "user-1";

        [SetUp]
        public void SetUp()
        {
            _articleServiceMock = new Mock<IArticleService>();
            _genreServiceMock = new Mock<IGenreService>();
            _commentServiceMock = new Mock<ICommentService>();
            _likeServiceMock = new Mock<ILikeService>();
            _imageStorageMock = new Mock<IImageStorageService>();

            _articleController = new ArticleController(
                _articleServiceMock.Object,
                _genreServiceMock.Object,
                _commentServiceMock.Object,
                _likeServiceMock.Object,
                _imageStorageMock.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId)
            }, "TestAuth"));

            _articleController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = user
                }
            };
        }

        [Test]
        public async Task ArticleCreateGet_ShouldReturnView_WithGenres()
        {
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Create();

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(view.Model, Is.InstanceOf<ArticleViewModel>());
        }

        [Test]
        public async Task ArticleCreatePost_ShouldCreateArticleAndRedirect_WhenValid()
        {
            var articleViewModel = new ArticleViewModel
            {
                Title = "testtitle",
                Content = "testcontent",
                GenreId = Guid.NewGuid()
            };
            _imageStorageMock.Setup(s => s.SaveArticleImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("/img/test.png");

            var result = await _articleController.Create(articleViewModel);

            _articleServiceMock.Verify(s =>
                s.CreateArticleAsync(
                    articleViewModel.Title,
                    "/img/test.png",
                    articleViewModel.Content,
                    articleViewModel.GenreId),
                Times.Once);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Blog"));
        }

        [Test]
        public async Task ArticleCreatePost_ShouldThrowsException_WhenImageException()
        {
            var articleViewModel = new ArticleViewModel
            {
                Title = "testtitle",
                Content = "testcontent",
                GenreId = Guid.NewGuid()
            };
            _imageStorageMock.Setup(s => s.SaveArticleImageAsync(It.IsAny<IFormFile>())).ThrowsAsync(new Exception());
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Create(articleViewModel);

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(_articleController.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task ArticleCreatePost_ShouldReturnView_WhenModelStateInvalid()
        {
            var articleViewModel = new ArticleViewModel
            {
                Title = "",
                Content = "testontent",
                GenreId = Guid.NewGuid()
            };
            _articleController.ModelState.AddModelError("Title", "Required");
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Create(articleViewModel);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(articleViewModel));
            _imageStorageMock.Verify(s => s.SaveArticleImageAsync(It.IsAny<IFormFile>()), Times.Never);
            _articleServiceMock.Verify(
                s => s.CreateArticleAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()),
                Times.Never);
        }

        [Test]
        public async Task ArticleDetailsGet_ShouldReturnNotFound_WhenArticleNotExists()
        {
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Article)null);

            var result = await _articleController.Details(Guid.NewGuid());

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task ArticleDetailsGet_ShouldRedirectToDetails_WhenArticleExists()
        {
            var article = new Article("testtitle", "/img/test.png", "testcontent", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _articleController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await _articleController.Details(article.ArticleId);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var articleViewModel = viewResult.Model as ArticleViewModel;
            Assert.That(articleViewModel, Is.Not.Null);
            Assert.That(articleViewModel.Title, Is.EqualTo(article.Title));
            Assert.That(articleViewModel.Content, Is.EqualTo(article.Content));
            Assert.That(articleViewModel.Image, Is.EqualTo(article.Image));
            Assert.That(articleViewModel.UserHasLiked, Is.False);
        }

        [Test]
        public async Task ArticleDetailsGet_ShouldRedirectToDetails_WhenUserAuthenticated()
        {
            var articleId = Guid.NewGuid();
            var userId = "user-1";
            var article = new Article("testtitle", "/img/test.png", "testcontent", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleId)).ReturnsAsync(article);

            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId) },
                "TestAuth"
            ));

            _articleController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _articleController.Details(articleId);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var articleViewModel = viewResult.Model as ArticleViewModel;
            Assert.That(articleViewModel, Is.Not.Null);
        }

        [Test]
        public async Task ArticleEditGet_ShouldThrowsException_WhenArticleExists()
        {
            var article = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());

            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Edit(article.ArticleId);

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
        }

        [Test]
        public async Task ArticleEditPost_ShouldUpdateArticle_WithoutNewImage()
        {
            var article = new Article("oldtitle", "/img/old.png", "oldcontent", Guid.NewGuid());
            var articleViewModel = new ArticleViewModel
            {
                ArticleViewModelId = article.ArticleId,
                Title = "newtitle",
                Content = "newcontent",
                GenreId = article.GenreId,
                ImageFile = null
            };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            var result = await _articleController.Edit(articleViewModel);

            _articleServiceMock.Verify(s =>
                s.UpdateArticleAsync(
                    article.ArticleId,
                    articleViewModel.Title,
                    article.Image,
                    articleViewModel.Content,
                    articleViewModel.GenreId),
                Times.Once);
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Blog"));
        }

        [Test]
        public async Task ArticleEditPost_ShouldUpdateArticle_WithNewImage()
        {
            var article = new Article("oldtitle", "/img/old.png", "oldcontent", Guid.NewGuid());
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(10);
            var articleViewModel = new ArticleViewModel
            {
                ArticleViewModelId = article.ArticleId,
                Title = "newtitle",
                Content = "newcontent",
                GenreId = article.GenreId,
                ImageFile = fileMock.Object
            };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);
            _imageStorageMock.Setup(s => s.SaveArticleImageAsync(fileMock.Object)).ReturnsAsync("/img/new.png");

            var result = await _articleController.Edit(articleViewModel);

            _imageStorageMock.Verify(s => s.SaveArticleImageAsync(fileMock.Object), Times.Once);
            _articleServiceMock.Verify(s =>
                s.UpdateArticleAsync(
                    article.ArticleId,
                    articleViewModel.Title,
                    "/img/new.png",
                    articleViewModel.Content,
                    articleViewModel.GenreId),
                Times.Once);
            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Blog"));
        }

        [Test]
        public async Task ArticleEditPost_ShouldReturnView_WhenArticleNotFound()
        {
            var articleViewModel = new ArticleViewModel
            {
                ArticleViewModelId = Guid.NewGuid(),
                Title = "testtitle",
                Content = "testcontent",
                GenreId = Guid.NewGuid()
            };
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(articleViewModel.ArticleViewModelId)).ReturnsAsync((Article)null);
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Edit(articleViewModel);

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            Assert.That(_articleController.ModelState.IsValid, Is.False);
        }

        [Test]
        public async Task ArticleEditPost_ShouldReturnView_WhenModelStateInvalid()
        {
            var articleViewModel = new ArticleViewModel();
            _articleController.ModelState.AddModelError("Title", "Required");
            _genreServiceMock.Setup(s => s.GetAllGenresAsync()).ReturnsAsync(new List<Genre>());

            var result = await _articleController.Edit(articleViewModel);

            var view = result as ViewResult;
            Assert.That(view, Is.Not.Null);
            _articleServiceMock.Verify(
                s => s.UpdateArticleAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Guid>()),
                Times.Never);
        }

        [Test]
        public async Task ArticleDeletePost_ShouldDeleteArticle_WithCommentsAndLikes()
        {
            var article = new Article("testtitle", "testimage", "testcontent", Guid.NewGuid());
            _articleServiceMock.Setup(s => s.GetArticleByIdAsync(article.ArticleId)).ReturnsAsync(article);

            var result = await _articleController.Delete(article.ArticleId);

            _commentServiceMock.Verify(s => s.DeleteCommentsByArticleIdAsync(article.ArticleId), Times.Once);
            _likeServiceMock.Verify(s => s.DeleteLikesByArticleIdAsync(article.ArticleId), Times.Once);
            _articleServiceMock.Verify(s => s.DeleteArticleAsync(article.ArticleId), Times.Once);
            Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        }

        [TearDown]
        public void TearDown()
        {
            if (_articleController != null)
            {
                _articleController.Dispose();
            }
        }
    }
}
