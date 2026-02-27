using BlogWebApp.Services.CommentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Guid ArticleId, string Content)
        {
            if (string.IsNullOrWhiteSpace(Content))
            {
                TempData["CommentError"] = "Комментарий не может быть пустым.";
                return RedirectToAction("Details", "Article", new { id = ArticleId });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _commentService.CreateCommentAsync(ArticleId, userId, Content);

            return RedirectToAction("Details", "Article", new { id = ArticleId });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid commentId, Guid articleId)
        {
            await _commentService.DeleteCommentByIdAsync(commentId);
            return RedirectToAction("Details", "Article", new { id = articleId });
        }
    }
}
