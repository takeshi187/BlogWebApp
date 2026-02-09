using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;

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
            var identityResult = IdentityResult.Failed(new IdentityError { Description = "Email already exists" });
            _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password")).ReturnsAsync(identityResult);

            var result = await _userService.RegisterAsync("username", "test@example.com", "password");

            Assert.That(result.Succeeded, Is.False);
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Once);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_ShouldThrowArgumentException_WhenInputInvalid()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.RegisterAsync("", "", ""));
            _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ApplicationUser>(), "password"), Times.Never);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnSuccess_WhenPasswordIsCorrect()
        {
            var user = new ApplicationUser { Email = "test@example.com", UserName = "testuser" };

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _signInManagerMock
                .Setup(m => m.PasswordSignInAsync(user, "password", true, false))
                .ReturnsAsync(SignInResult.Success);

            var result = await _userService.LoginAsync(user.Email, "password", true);

            Assert.That(result, Is.True);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(user.Email), Times.Once);
            _signInManagerMock.Verify(m => m.PasswordSignInAsync(user, "password", true, false), Times.Once);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            var result = await _userService.LoginAsync("test@example.com", "password", true);

            Assert.That(result, Is.False);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_ShouldThrowArgumentException_WhenInputInvalid()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.LoginAsync("", "", false));
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnFalse_WhenPasswordIncorrect()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);
            _signInManagerMock
                .Setup(m => m.PasswordSignInAsync(user, "wrong", true, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _userService.LoginAsync(user.Email, "wrong", true);

            Assert.That(result, Is.False);
            _userRepositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Once);
            _signInManagerMock.Verify(m => m.PasswordSignInAsync(user, "wrong", true, false), Times.Once);
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
        public async Task GetUserByEmailAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.GetUserByEmailAsync("test@example.com"));
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
        public async Task GetUserByUserIdAsync_ShouldThrowInvalidOperationException_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _userService.GetUserByIdAsync("1"));
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
            _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldThrowArgumentException_WhenUserIdEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(async () =>
                await _userService.DeleteUserAsync(""));
            _userRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Test]
        public async Task LogOutAsync_ShouldSignOut_WhenValid()
        {
            await _userService.LogoutAsync();
            _signInManagerMock.Verify(s => s.SignOutAsync(), Times.Once);
        }
    }
}
