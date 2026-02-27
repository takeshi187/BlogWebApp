using BlogWebApp.Controllers;
using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.Services.UserServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace BlogWebApp.Tests.ControllerTests
{
    [TestFixture]
    public class AdminControllerTests
    {
        private Mock<IUserService> _userServiceMock;
        private Mock<ILikeService> _likeServiceMock;
        private Mock<ICommentService> _commentServiceMock;
        private AdminController _adminController;

        [SetUp]
        public void SetUp()
        {
            _userServiceMock = new Mock<IUserService>();
            _likeServiceMock = new Mock<ILikeService>();
            _commentServiceMock = new Mock<ICommentService>();
            _adminController = new AdminController(_userServiceMock.Object, _likeServiceMock.Object, _commentServiceMock.Object);
        }

        [Test]
        public async Task AdminIndexGet_ShouldReturnView_WhenValid()
        {
            var user1 = new ApplicationUser() { UserName = "Test", Email = "test@example.com" } ;
            var user2 = new ApplicationUser() { UserName = "Test1", Email = "test1@example.com" } ;
            var users = new List<ApplicationUser>();
            users.Add(user1);
            users.Add(user2);
            _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await _adminController.Index();

            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            var model = viewResult.Model as List<UserViewModel>;
            Assert.That(model, Is.Not.Null);
            Assert.That(model.Count, Is.EqualTo(2));
            Assert.That(model[0].UserName, Is.EqualTo("Test"));
            Assert.That(model[0].Email, Is.EqualTo("test@example.com"));
            Assert.That(model[1].UserName, Is.EqualTo("Test1"));
            Assert.That(model[1].Email, Is.EqualTo("test1@example.com"));
            _userServiceMock.Verify(s => s.GetAllUsersAsync(), Times.Once);
        }

        [Test]
        public async Task AdminDeleteUserPost_ShouldReturnBadRequest_WhenUserIdIsNullOrEmpty()
        {
            string userId = null;

            var result = await _adminController.DeleteUser(userId);

            Assert.That(result, Is.TypeOf<BadRequestResult>());
            _userServiceMock.Verify(s => s.DeleteUserAsync(userId), Times.Never);
        }

        [Test]
        public async Task DeleteUserPost_ShouldReturnNotFound_WhenInvalid()
        {
            var userId = "1";
            _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(false);

            var result = await _adminController.DeleteUser(userId);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
            _likeServiceMock.Verify(s => s.DeleteLikesByUserIdAsync(userId), Times.Never);
            _commentServiceMock.Verify(s => s.DeleteCommentsByUserIdAsync(userId), Times.Never);
        }

        [Test]
        public async Task AdminDeleteUserPost_ShouldReturnRedirectToAction_WhenValid()
        {
            var userId = "1";
            _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).ReturnsAsync(true);

            var result = await _adminController.DeleteUser(userId);

            var redirectToActionResult = result as RedirectToActionResult;
            Assert.That(redirectToActionResult, Is.Not.Null);
            Assert.That("Index", Is.EqualTo(redirectToActionResult.ActionName));
            _userServiceMock.Verify(s => s.DeleteUserAsync(userId), Times.Once);
            _likeServiceMock.Verify(s => s.DeleteLikesByUserIdAsync(userId), Times.Once);
            _commentServiceMock.Verify(s => s.DeleteCommentsByUserIdAsync(userId), Times.Once);
        }

        [Test]
        public void AdminController_ShouldHaveAuthorizeAttribute_WithRolesAdmin()
        {
            var attribute = typeof(AdminController).GetCustomAttributes(typeof(AuthorizeAttribute), false)
                .FirstOrDefault() as AuthorizeAttribute;

            Assert.That(attribute, Is.Not.Null);
            Assert.That("Admin", Is.EqualTo(attribute.Roles));
        }

        [TearDown]
        public void TearDown()
        {
            if (_adminController != null)
            {
                _adminController.Dispose();
            }
        }
    }
}
