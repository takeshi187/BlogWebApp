using BlogWebApp.Db;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.LikeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;
        private readonly BlogWebAppDbContext _db;
        public LikeController(BlogWebAppDbContext db, ILikeService likeService)
        {
            _db = db;
            _likeService = likeService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(Guid articleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _likeService.ToggleLikeAsync(articleId, userId);

            return RedirectToAction("Index", "Blog");
        }
    }
}
