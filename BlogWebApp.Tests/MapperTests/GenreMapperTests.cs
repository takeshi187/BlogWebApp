using BlogWebApp.Mappers;
using BlogWebApp.Models;

namespace BlogWebApp.Tests.MapperTests
{
    [TestFixture]
    public class GenreMapperTests
    {
        [Test]
        public void GenreMapperToViewModel_ShouldMapGenre_WhenValid()
        {
            var genre = new Genre("testgenre");

            var result = GenreMapper.ToViewModel(genre);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.GenreId, Is.EqualTo(genre.GenreId));
            Assert.That(result.GenreName, Is.EqualTo(genre.GenreName));
        }
    }
}
