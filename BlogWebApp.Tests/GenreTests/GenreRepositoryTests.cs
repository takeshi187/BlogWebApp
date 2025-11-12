using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.GenreServices;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Tests.GenreTests
{
    [TestFixture]
    public class GenreRepositoryTests
    {
        private GenreRepository _genreRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase("TestDatabase").Options;

            _db = new BlogWebAppDbContext(options);
            _genreRepository = new GenreRepository(_db);
        }

        [Test]
        public async Task AddGenreAsync_ShouldAddGenre_WhenValid()
        {
            var genre = new Genre(Guid.NewGuid(), "testname");

            var result = await _genreRepository.AddAsync(genre);

            Assert.That(result.GenreId, Is.EqualTo(genre.GenreId));
            Assert.That(result.GenreName, Is.EqualTo(genre.GenreName));
        }

        [Test]
        public async Task GetGenreByIdAsync_ShouldReturnGenre_WhenGenreExist()
        {
            var genre = new Genre(Guid.NewGuid(), "testname");

            await _genreRepository.AddAsync(genre);
            var result = await _genreRepository.GetByIdAsync(genre.GenreId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GenreId, Is.EqualTo(genre.GenreId));
        }

        [Test]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres_WhenGenresExist()
        {
            var genre1 = new Genre(Guid.NewGuid(), "testgenre1");
            var genre2 = new Genre(Guid.NewGuid(), "testgenre2");
            var genre3 = new Genre(Guid.NewGuid(), "testgenre3");

            _db.Genres.RemoveRange(_db.Genres);
            await _db.SaveChangesAsync();
            await _genreRepository.AddAsync(genre1);
            await _genreRepository.AddAsync(genre2);
            await _genreRepository.AddAsync(genre3);

            var result = await _genreRepository.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [TearDown]
        public void TearDown()
        {
            if (_db != null)
            {
                _db.Dispose();
            }
        }
    }
}
