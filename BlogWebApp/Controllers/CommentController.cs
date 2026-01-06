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
                ModelState.AddModelError("", "Комментарий не может быть пустым.");
                return RedirectToAction("Details", "Article", new { id = articleId});
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var comment = new Comment(content, userId, articleId);

            await _commentService.CreateCommentAsync(comment);

            return RedirectToAction("Details", "Article", new {id = articleId});
        }
    }
}
