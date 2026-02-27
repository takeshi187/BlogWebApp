using BlogWebApp.Mappers;
using BlogWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests.MapperTests
{
    [TestFixture]
    public class UserMapperTests
    {
        [Test]
        public void UserToViewModel_ShouldMapUser_WhenValid()
        {
            var user = new ApplicationUser()
            {
                UserName = "Maxx",
                Email = "maxx@test.com",
            };

            var result = UserMapper.ToViewModel(user);

            Assert.That(result.Id, Is.EqualTo(user.Id));
            Assert.That(result.UserName, Is.EqualTo(user.UserName));
            Assert.That(result.Email, Is.EqualTo(user.Email));
            Assert.That(result.CommentsCount, Is.EqualTo(0));
            Assert.That(result.LikesCount, Is.EqualTo(0));
        }
    }
}
