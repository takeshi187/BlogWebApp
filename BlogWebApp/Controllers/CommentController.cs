using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.CommentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    public class CommentController : Controller
    {
        private readonly BlogWebAppDbContext _db;
        private readonly ICommentService _commentService;

        public CommentController(BlogWebAppDbContext db, ICommentService commentService)
        {
            _db = db;
            _commentService = commentService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(Guid articleId, string content)
        {
            if(string.IsNullOrWhiteSpace(content))
            {
                TempData["Error"] = "Комментарий не может быть пустым.";
                return RedirectToAction("Details", "Article", new { id = articleId});
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentService.CreateCommentAsync(articleId, userId, content);

            if (result == null)
            {
                TempData["Error"] = "Не удалось добавить комментарий.";
            }

            return RedirectToAction("Details", "Article", new {id = articleId});
        }
    }
}
