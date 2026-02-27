using BlogWebApp.Models;
using BlogWebApp.Services.GenreServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace BlogWebApp.Tests.GenreTests
{
    [TestFixture]
    public class GenreServiceTests
    {
        private Mock<IGenreRepository> _genreRepositoryMock;
        private Mock<ILogger<GenreService>> _loggerMock;
        private GenreService _genreService;

        [SetUp]
        public void SetUp()
        {
            _genreRepositoryMock = new Mock<IGenreRepository>();
            _loggerMock = new Mock<ILogger<GenreService>>();
            _genreService = new GenreService(_genreRepositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldCreateGenre_WhenValid()
        {
            await _genreService.CreateGenreAsync("testname");

            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldThrowArgumentException_WhenGenreNameIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _genreService.CreateGenreAsync(string.Empty));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Never);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldThrowDbUpdateException_WhenGenreIsAlreadyExist()
        {
            _genreRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Genre>()))
                .ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<DbUpdateException>(async () =>
                await _genreService.CreateGenreAsync("testname"));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldReturnFalse_WhenGenreAlreadyExists()
        {
            var name = "Rock";
            _genreRepositoryMock.Setup(r => r.GetByNameAsync(name)).ReturnsAsync(new Genre(name));

            var result = await _genreService.CreateGenreAsync(name);

            Assert.That(result, Is.False);
            _genreRepositoryMock.Verify(r => r.GetByNameAsync(name), Times.Once);
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Never);
        }

        [Test]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres_WhenGenresExist()
        {
            var genre1 = new Genre("testname1");
            var genre2 = new Genre("testname2");
            var genres = new List<Genre> { genre1, genre2 };
            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            var result = await _genreService.GetAllGenresAsync();

            Assert.That(genres, Is.EqualTo(result));
            _genreRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public void GetAllGenresAsync_ShouldThrowException_WhenInvalid()
        {
            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("DB failure"));

            Assert.ThrowsAsync<Exception>(async () =>
                await _genreService.GetAllGenresAsync());
            _genreRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
