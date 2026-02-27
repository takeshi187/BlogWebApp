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

        [Test]
        public async Task GetByNameAsync_ShouldReturnGenre_WhenExist()
        {
            var genre = new Genre("testgenre1");
            await _genreRepository.AddAsync(genre);

            var result = await _genreRepository.GetByNameAsync(genre.GenreName);

            Assert.That(result, Is.EqualTo(genre));
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
