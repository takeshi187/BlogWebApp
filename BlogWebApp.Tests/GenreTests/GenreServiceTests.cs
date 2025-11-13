using BlogWebApp.Models;
using BlogWebApp.Services.GenreServices;
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
            var result = await _genreService.CreateGenreAsync("testname");
            _genreRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Genre>())).ReturnsAsync(result);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GenreName, Is.EqualTo("testname"));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldThrowArgumentException_WhenGenreNameIsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _genreService.CreateGenreAsync(string.Empty));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Never);
        }

        [Test]
        public async Task CreateGenreAsync_ShouldThrowInvalidOperationException_WhenGenreIsAlreadyExist()
        {
            var genreName = "testname";
            var existingGenre = new Genre(Guid.NewGuid(), genreName);
            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Genre> { existingGenre });

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _genreService.CreateGenreAsync(genreName));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Never);
        }

        [Test]
        public async Task GetGenreById_ShouldReturnGenre_WhenGenreExist()
        {
            var genre = new Genre(Guid.NewGuid(), "testname");
            _genreRepositoryMock.Setup(r => r.GetByIdAsync(genre.GenreId)).ReturnsAsync(genre);

            var result = await _genreService.GetGenreByIdAsync(genre.GenreId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GenreId, Is.EqualTo(genre.GenreId));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(genre.GenreId), Times.Once);
        }

        [Test]
        public async Task GetGenreById_ShouldThrowInvalidOperationException_WhenGenreNotFound()
        {
            var genre = new Genre(Guid.NewGuid(), "testname");
            _genreRepositoryMock.Setup(r => r.GetByIdAsync(genre.GenreId)).ReturnsAsync((Genre?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _genreService.GetGenreByIdAsync(genre.GenreId));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(genre.GenreId), Times.Once);
        }

        [Test]
        public async Task GetGenreById_ShouldThrowArgumentException_WhenGenreEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () => await _genreService.GetGenreByIdAsync(Guid.Empty));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Never);
        }

        [Test]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres_WhenGenresExist()
        {
            var genre1 = new Genre(Guid.NewGuid(), "testname1");
            var genre2 = new Genre(Guid.NewGuid(), "testname2");
            var genres = new List<Genre> { genre1, genre2 };
            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            var result = await _genreService.GetAllGenresAsync();

            Assert.That(genres, Is.EqualTo(result));
            _genreRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllGenresAsync_ShouldThrowInvalidOperationException_WhenGenresNotFound()
        {
            _genreRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Genre>());

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _genreService.GetAllGenresAsync());
            _genreRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }
    }
}
