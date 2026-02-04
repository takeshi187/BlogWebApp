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
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _db = new BlogWebAppDbContext(options);
            _genreRepository = new GenreRepository(_db);
        }

        [Test]
        public async Task AddGenreAsync_ShouldAddGenre_WhenValid()
        {
            var genre = new Genre("testname");

            await _genreRepository.AddAsync(genre);
            var result = await _genreRepository.GetByNameAsync("testname");

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GenreName, Is.EqualTo(genre.GenreName));
        }

        [Test]
        public async Task GetGenreByIdAsync_ShouldReturnGenre_WhenGenreExist()
        {
            var genre = new Genre("testname");

            await _genreRepository.AddAsync(genre);
            var result = await _genreRepository.GetByIdAsync(genre.GenreId);

            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task GetAllGenresAsync_ShouldReturnAllGenres_WhenGenresExist()
        {
            var genre1 = new Genre("testgenre1");
            var genre2 = new Genre("testgenre2");
            var genre3 = new Genre("testgenre3");

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
