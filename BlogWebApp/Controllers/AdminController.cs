using BlogWebApp.Mappers;
using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.Services.UserServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILikeService _likeService;
        private readonly ICommentService _commentService;

        public AdminController(
            IUserService userService,
            ILikeService likeService,
            ICommentService commentService)
        {
            _userService = userService;
            _likeService = likeService;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            var userViewModel = users
                .Select(UserMapper.ToViewModel)
                .ToList();

            return View(userViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
                return NotFound();

            await _likeService.DeleteLikesByUserIdAsync(userId);
            await _commentService.DeleteCommentsByUserIdAsync(userId);

            return RedirectToAction(nameof(Index));
        }
    }
}
