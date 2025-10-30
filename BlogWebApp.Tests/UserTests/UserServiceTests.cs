using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogWebApp.Tests.UserTests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private UserService _userService;

        [SetUp]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();

            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
                userManagerMock.Object, contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _userService = new UserService(_userRepositoryMock.Object, _signInManagerMock.Object);
        }

        [Test]
        public async Task RegisterAsync_ShouldReturnFailed_WhenUserAlreadyExist()
        {
            var existingUser = new ApplicationUser { Email = "test@example.com" };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);

            var result = await _userService.RegisterAsync("username", "test@example.com", "password");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.One.Matches<IdentityError>(e => e.Description.Contains("User already exist")));
        }

        [Test]
        public async Task RegisterAsync_ShouldReturnSuccess_WhenUserDoesNotExist()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser)null);

            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userService.RegisterAsync("username", "test@example.com", "password");

            Assert.That(result.Succeeded, Is.True);
        }

        [Test]
        public async Task LoginAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByEmailAsync("test@example.com"))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _userService.LoginAsync("test@example.com", "password");

            Assert.That(result, Is.False);
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
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnFailed_WhenUserNotFound()
        {
            _userRepositoryMock.Setup(r => r.GetByIdAsync("1"))
                .ReturnsAsync((ApplicationUser)null);

            var result = await _userService.DeleteUserAsync("1");

            Assert.That(result.Succeeded, Is.False);
            Assert.That(result.Errors, Has.One.Matches<IdentityError>(e => e.Description.Contains("User not found")));
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
    }   
}
