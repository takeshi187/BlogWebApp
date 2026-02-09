using BlogWebApp.Controllers;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class GenreControllerTests
    {
        private Mock<IGenreService> _genreServiceMock;
        private GenreController _genreController;

        [SetUp]
        public void SetUp()
        {
            _genreServiceMock = new Mock<IGenreService>();
            _genreController = new GenreController(_genreServiceMock.Object);
        }

        [Test]
        public async Task CreateGenreGet_ShouldRedirectToGenreCreatePage()
        {
            var result = _genreController.Create();
            var redirect = result as ViewResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.Model, Is.TypeOf<GenreViewModel>());
        }

        [Test]
        public async Task CreateGenrePost_ShouldCreateGenreAndRedirect_WhenValid()
        {
            var genreViewModel = new GenreViewModel
            {
                GenreName = "testgenre"
            };
            _genreServiceMock.Setup(s => s.CreateGenreAsync(genreViewModel.GenreName)).Returns(Task.CompletedTask);

            var result = await _genreController.Create(genreViewModel);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect!.ActionName, Is.EqualTo("Create"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Article"));
            _genreServiceMock.Verify(s => s.CreateGenreAsync(genreViewModel.GenreName), Times.Once);
        }

        [Test]
        public async Task CreateGenrePost_ShouldThrowDuplicateException_WhenGenreAlreadyExist()
        {
            var genreViewModel = new GenreViewModel
            {
                GenreName = "testgenre"
            };
            _genreServiceMock.Setup(s => s.CreateGenreAsync(genreViewModel.GenreName)).ThrowsAsync(new Exception("Duplicate genre"));

            var result = await _genreController.Create(genreViewModel);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult!.Model, Is.EqualTo(genreViewModel));
            Assert.That(_genreController.ModelState.ContainsKey(nameof(genreViewModel.GenreName)), Is.True);
        }

        [TearDown]
        public void TearDown()
        {
            if (_genreController != null)
            {
                _genreController.Dispose();
            }
        }
    }
}
