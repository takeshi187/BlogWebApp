using BlogWebApp.Mappers;
using BlogWebApp.Models;

namespace BlogWebApp.Tests.MapperTests
{
    [TestFixture]
    public class CommentMapperTests
    {
        [Test]
        public void ToViewModel_ValidComment_MapsCorrectly()
        {
            var user = new ApplicationUser { Id = "user1", UserName = "user1" };
            var comment = new Comment("testcontent", user, Guid.NewGuid());

            var result = CommentMapper.ToViewModel(comment);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserName, Is.EqualTo("user1"));
        }
    }
}
