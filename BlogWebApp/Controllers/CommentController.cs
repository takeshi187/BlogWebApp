using BlogWebApp.Mappers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.ViewModels;
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

            try
            {
                await _commentService.CreateCommentAsync(ArticleId, userId, Content);
            }
            catch
            {
                TempData["CommentError"] = "Не удалось добавить комментарий.";
            }

            return RedirectToAction("Details", "Article", new { id = ArticleId });
        }
    }
}
