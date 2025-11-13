using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BlogWebApp.Tests.UserTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<ILogger<UserService>> _loggerMock;
        private Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private UserService _userService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<UserService>>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _userService = new UserService(_userRepositoryMock.Object, _signInManagerMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.RegisterAsync("username", "test@example.com", "password");

            Assert.That(result.Succeeded, Is.True);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Once);
        }

        [Test]
        public async Task RegisterUserAsync_ShouldReturnFailed_WhenUserAlreadyExist()
        {
            var existingUser = new ApplicationUser { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);

            var result = await _userService.RegisterAsync("username", "test@example.com", "password");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.One.Matches<IdentityError>(e => e.Description.Contains("User already exist")));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public async Task RegisterUserAsync_ShouldThrowArgumentException_WhenUsernameEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.RegisterAsync("", "test@example.com", "password"));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public void RegisterAsync_ShouldThrowArgumentException_WhenEmailEmpty()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.RegisterAsync("username", "", "password"));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public void RegisterAsync_ShouldThrowArgumentException_WhenPasswordEmpty()
        {
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.RegisterAsync("username", "test@example.com", ""));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public void RegisterAsync_ShouldThrowInvalidOperationException_WhenFailed()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
               .ThrowsAsync(new InvalidOperationException("Test error"));

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _userService.RegisterAsync("user", "a@a.com", "123"));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnTrue_WhenPasswordIsCorrect()
        {
            var user = new ApplicationUser { Email = "test@example.com" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(m => m.PasswordSignInAsync(user, "password", false, false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _userService.LoginAsync(user.Email, "password");

            Assert.That(result, Is.True);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(user.Email), Times.Once);
        }

        [Test]
        public async Task LoginAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.LoginAsync("test@example.com", "password"));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync("test@example.com"), Times.Once);
        }

        [Test]
        public async Task LoginAsync_ShouldThrowArgumentException_WhenEmailEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.LoginAsync("", "password"));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(""), Times.Never);
        }

        [Test]
        public async Task LoginAsync_ShouldThrowArgumentException_WhenPasswordEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.LoginAsync("test@example.com", ""));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync("test@example.com"), Times.Never);
        }

        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenUserExist()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var result = await _userService.GetUserByEmailAsync(user.Email);

            Assert.That(result, Is.Not.Null);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(user.Email), Times.Once);
        }

        [Test]
        public void GetUserByEmailAsync_ShouldThrowArgumentException_WhenEmailEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByEmailAsync(""));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(""), Times.Never);
        }

        [Test]
        public void GetUserByEmailAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.GetUserByEmailAsync("test@example.com"));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync("test@example.com"), Times.Once);
        }

        [Test]
        public async Task GetUserByUserIdAsync_ShouldReturnUser_WhenUserExist()
        {
            var user = new ApplicationUser { Id = "1" };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            var result = await _userService.GetUserByIdAsync(user.Id);

            Assert.That(result, Is.Not.Null);
            _userRepositoryMock.Verify(r => r.GetByIdAsync(user.Id), Times.Once);
        }

        [Test]
        public void GetUserByUserIdAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _userService.GetUserByIdAsync(""));
            _userRepositoryMock.Verify(r => r.GetByIdAsync(""), Times.Never);
        }

        [Test]
        public void GetUserByUserIdAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(() => _userService.GetUserByIdAsync("1"));
            _userRepositoryMock.Verify(r => r.GetByIdAsync("1"), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExist()
        {
            var user = new ApplicationUser { Id = "1" };

            _userRepositoryMock.Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(r => r.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.DeleteUserAsync("1");

            Assert.That(result.Succeeded, Is.True);
            _userRepositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFailed_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await _userService.DeleteUserAsync("1");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.One.Matches<IdentityError>(e => e.Description.Contains("User not found")));
            _userRepositoryMock.Verify(r => r.DeleteAsync(null), Times.Never);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _userService.DeleteUserAsync(""));
            _userRepositoryMock.Verify(r => r.DeleteAsync(null), Times.Never);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<string>()))
               .ThrowsAsync(new InvalidOperationException("Test error"));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.DeleteUserAsync("1"));
            _userRepositoryMock.Verify(r => r.DeleteAsync(null), Times.Never);
        }

        [Test]
        public async Task LogOutAsync_ShouldSignOut_WhenValid()
        {
            await _userService.LogoutAsync();
            _signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
        }
    }
}
