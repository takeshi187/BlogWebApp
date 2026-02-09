using BlogWebApp.Mappers;
using BlogWebApp.Models;

namespace BlogWebApp.Tests.MapperTests
{
    [TestFixture]
    public class ArticleMapperTests
    {
        [Test]
        public void ArticleToViewModel_ShouldMapArticle_WhenValid()
        {
            var article = new Article("testtitle", "testcontent", "testimage", Guid.NewGuid());

            var result = ArticleMapper.ToViewModel(article);

            Assert.That(result.ArticleViewModelId, Is.EqualTo(article.ArticleId));
            Assert.That(result.Title, Is.EqualTo(article.Title));
            Assert.That(result.Content, Is.EqualTo(article.Content));
            Assert.That(result.Image, Is.EqualTo(article.Image));
            Assert.That(result.GenreId, Is.EqualTo(article.GenreId));
            Assert.That(result.CreatedAt, Is.EqualTo(article.CreatedAt));
            Assert.That(result.UpdatedAt, Is.EqualTo(article.UpdatedAt));
            Assert.That(result.LikesCount, Is.EqualTo(0));
            Assert.That(result.CommentsCount, Is.EqualTo(0));
            Assert.That(result.Comments, Is.Not.Null);
            Assert.That(result.Comments.Count, Is.EqualTo(0));
            Assert.That(result.UserHasLiked, Is.False);
        }
    }
}
