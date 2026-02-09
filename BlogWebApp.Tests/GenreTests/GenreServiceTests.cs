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
        public async Task CreateGenreAsync_ShouldThrowInvalidOperationException_WhenGenreIsAlreadyExist()
        {
            _genreRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Genre>()))
                .ThrowsAsync(new DbUpdateException());

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _genreService.CreateGenreAsync("testname"));
            _genreRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Test]
        public async Task GetGenreById_ShouldReturnGenre_WhenGenreExist()
        {
            var genre = new Genre("testname");
            _genreRepositoryMock.Setup(r => r.GetByIdAsync(genre.GenreId)).ReturnsAsync(genre);

            var result = await _genreService.GetGenreByIdAsync(genre.GenreId);

            Assert.That(result, Is.EqualTo(genre));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(genre.GenreId), Times.Once);
        }

        [Test]
        public async Task GetGenreById_ShouldThrowInvalidOperationException_WhenGenreNotFound()
        {
            _genreRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Genre?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _genreService.GetGenreByIdAsync(Guid.NewGuid()));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Test]
        public async Task GetGenreById_ShouldThrowArgumentException_WhenGenreEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _genreService.GetGenreByIdAsync(Guid.Empty));
            _genreRepositoryMock.Verify(r => r.GetByIdAsync(Guid.Empty), Times.Never);
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
    }
}
