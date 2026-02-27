using BlogWebApp.Controllers;
using BlogWebApp.Services.UserServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private AccountController _accountController;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _accountController = new AccountController(_userServiceMock.Object);
        }

        [Test]
        public async Task AccountRegisterPost_ShouldReturnView_WhenModelStateInvalid()
        {
            var registerViewModel = new RegisterViewModel
            {
                UserName = "",
                Email = "test@test.com",
                Password = "123"
            };
            _accountController.ModelState.AddModelError("UserName", "Required");

            var result = await _accountController.Register(registerViewModel);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(viewResult.Model, Is.EqualTo(registerViewModel));
            _userServiceMock.Verify(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task AccountRegisterPost_ShouldRegisterAccountAndRedirect_WhenValid()
        {
            var registerViewModel = new RegisterViewModel
            {
                UserName = "user1",
                Email = "user@test.com",
                Password = "Password1!"
            };
            _userServiceMock.Setup(s => s.RegisterAsync(registerViewModel.UserName, registerViewModel.Email, registerViewModel.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _accountController.Register(registerViewModel);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            _userServiceMock.Verify(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task AccountRegisterPost_ShouldAddModelError_WhenDuplicateUserName()
        {
            var registerViewModel = new RegisterViewModel
            {
                UserName = "user1",
                Email = "user@test.com",
                Password = "Password1!"
            };

            var identityError = new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Username already taken"
            };
            _userServiceMock.Setup(s => s.RegisterAsync(registerViewModel.UserName, registerViewModel.Email, registerViewModel.Password))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            var result = await _accountController.Register(registerViewModel);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(_accountController.ModelState.IsValid, Is.False);

            var error = _accountController.ModelState[string.Empty].Errors.First().ErrorMessage;
            Assert.That(error, Is.EqualTo("Имя пользователя уже занято."));
            _userServiceMock.Verify(s => s.RegisterAsync(registerViewModel.UserName, registerViewModel.Email, registerViewModel.Password), Times.Once);
        }

        [Test]
        public async Task AccountLoginPost_ShouldLoginAndRedirect_WhenValid()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "Password1!",
                RememberMe = true
            };
            _userServiceMock.Setup(s => s.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe))
                .ReturnsAsync(true);

            var result = await _accountController.Login(loginViewModel);

            var redirect = result as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            _userServiceMock.Verify(s => s.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe), Times.Once);
        }

        [Test]
        public async Task AccountLoginPost_ShouldReturnViewWithError_WhenInvalid()
        {
            var loginViewModel = new LoginViewModel
            {
                Email = "user@test.com",
                Password = "wrong",
                RememberMe = false
            };
            _userServiceMock.Setup(s => s.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe))
                .ReturnsAsync(false);

            var result = await _accountController.Login(loginViewModel);

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(_accountController.ModelState.IsValid, Is.False);
            var error = _accountController.ModelState[string.Empty].Errors.First().ErrorMessage;
            Assert.That(error, Is.EqualTo("Неверный логин или пароль."));
            _userServiceMock.Verify(s => s.LoginAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe), Times.Once);
        }

        [Test]
        public async Task AccountLogoutPost_ShouldCallServiceAndRedirect()
        {
            var result = await _accountController.Logout();
            var redirect = result as RedirectToActionResult;

            Assert.That(redirect.ActionName, Is.EqualTo("Index"));
            Assert.That(redirect.ControllerName, Is.EqualTo("Home"));
            _userServiceMock.Verify(s => s.LogoutAsync(), Times.Once);
        }

        [TearDown]
        public void TearDown()
        {
            if (_accountController != null)
            {
                _accountController.Dispose();
            }
        }
    }
}
