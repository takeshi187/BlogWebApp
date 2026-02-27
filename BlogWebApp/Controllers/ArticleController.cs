using BlogWebApp.Mappers;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.FileStorageServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BlogWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IGenreService _genreService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly IImageStorageService _imageStorageService;

        public ArticleController(
            IArticleService articleService,
            IGenreService genreService,
            ICommentService commentService,
            ILikeService likeService,
            IImageStorageService imageStorageService)
        {
            _articleService = articleService;
            _genreService = genreService;
            _commentService = commentService;
            _likeService = likeService;
            _imageStorageService = imageStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var articleViewModel = new ArticleViewModel();
            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {

                articleViewModel.Image = await _imageStorageService.SaveArticleImageAsync(articleViewModel.ImageFile);
                await _articleService.CreateArticleAsync(articleViewModel.Title,
                    articleViewModel.Image,
                    articleViewModel.Content,
                    articleViewModel.GenreId);
                return RedirectToAction("Index", "Blog");
            }

            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            return View(ArticleMapper.ToViewModel(article, userId));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            var articleViewModel = ArticleMapper.ToViewModel(article, userId);
            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                string? imagePath = articleViewModel.Image;
                if (articleViewModel.ImageFile != null && articleViewModel.ImageFile.Length > 0)
                    imagePath = await _imageStorageService.SaveArticleImageAsync(articleViewModel.ImageFile);

                var result = await _articleService.UpdateArticleAsync(
                    articleViewModel.ArticleViewModelId,
                    articleViewModel.Title,
                    imagePath,
                    articleViewModel.Content,
                    articleViewModel.GenreId
                );

                if (!result)
                    return NotFound();

                return RedirectToAction("Index", "Blog");
            }

            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _articleService.DeleteArticleAsync(id);

            if (!result)
                return NotFound();

            await _commentService.DeleteCommentsByArticleIdAsync(id);
            await _likeService.DeleteLikesByArticleIdAsync(id);

            return RedirectToAction("Index", "Blog");
        }

        private async Task LoadGenres(ArticleViewModel articleViewModel)
        {
            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres.Select(GenreMapper.ToViewModel).ToList();
        }
    }
}
