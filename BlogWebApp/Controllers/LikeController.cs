using BlogWebApp.Services.LikeServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class LikeController : Controller
    {
        private readonly ILikeService _likeService;
        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleLike(Guid articleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _likeService.ToggleLikeAsync(articleId, userId);

            return RedirectToAction("Details", "Article", new { id = articleId });
        }
    }
}
