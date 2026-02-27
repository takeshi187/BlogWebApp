using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BlogWebApp.Tests.UserTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private UserRepository _userRepository;
        private BlogWebAppDbContext _db;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<BlogWebAppDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _db = new BlogWebAppDbContext(options);
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _userRepository = new UserRepository(_userManagerMock.Object, _db);
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnIdentityResult_WhenValid()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userManagerMock.Setup(m => m.CreateAsync(user, "password")).ReturnsAsync(IdentityResult.Success);

            var result = await _userRepository.AddAsync(user, "password");

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task GetUserById_ShouldReturnUser_WhenUserExist()
        {
            var user = new ApplicationUser { Id = "1" };
            _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _userRepository.GetByIdAsync(user.Id);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task GetUserByEmail_ShouldReturnUser_WhenUserExist()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);

            var result = await _userRepository.GetByEmailAsync(user.Email);

            Assert.That(result, Is.EqualTo(user));
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnIdentityResult_WhenUserExist()
        {
            var user = new ApplicationUser { Id = "1" };
            _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _userRepository.DeleteAsync(user);

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnUsers_WhenUsersExist()
        {
            var user1 = new ApplicationUser { Id = "1" };
            var user2 = new ApplicationUser { Id = "2" };
            _db.Users.Add(user1);
            _db.Users.Add(user2);
            await _db.SaveChangesAsync();

            var result = await _userRepository.GetAllAsync();

            Assert.That(result.Count, Is.EqualTo(2));

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
