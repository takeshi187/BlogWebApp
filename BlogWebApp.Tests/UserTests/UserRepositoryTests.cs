using BlogWebApp.Models;
using BlogWebApp.Services.UserServices;
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
    public class UserRepositoryTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private UserRepository _userRepository;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _userRepository = new UserRepository(_userManagerMock.Object);
        }

        [Test]
        public async Task CreateUserAsync_ShouldReturnIdentityResult_WhenValid()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userManagerMock.Setup(m => m.CreateAsync(user, "password")).ReturnsAsync(IdentityResult.Success);

            var result = await _userRepository.CreateAsync(user, "password");

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
        public async Task CheckUserPassword_ShouldReturnTrue_WhenCorrect()
        {
            var user = new ApplicationUser { Email = "test@example.com" };
            _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "password")).ReturnsAsync(true);

            var result = await _userRepository.CheckPasswordAsync(user, "password");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteUserAsync_ShouldReturnIdentityResult_WhenUserExist()
        {
            var user = new ApplicationUser { Id = "1" };
            _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _userRepository.DeleteAsync(user);

            Assert.That(result.Succeeded, Is.True);
        }
    }
}
