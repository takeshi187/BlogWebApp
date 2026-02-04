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

namespace BlogWebApp.Controllers
{
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
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var articleViewModel = new ArticleViewModel();
            await LoadGenres(articleViewModel);         
            return View(articleViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    articleViewModel.Image = await _imageStorageService.SaveArticleImageAsync(articleViewModel.ImageFile);
                    await _articleService.CreateArticleAsync(articleViewModel.Title, 
                        articleViewModel.Image, 
                        articleViewModel.Content, 
                        articleViewModel.GenreId);
                    return RedirectToAction("Index", "Blog");
                }
                catch
                {
                    ModelState.AddModelError("", "Не удалось создать пост.");
                    await LoadGenres(articleViewModel);
                    return View(articleViewModel);
                }               
            }

            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

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
        [Authorize]
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
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                var article = await _articleService.GetArticleByIdAsync(articleViewModel.ArticleViewModelId);
                if (article == null)
                {
                    ModelState.AddModelError("", "Статья не найдена.");
                    await LoadGenres(articleViewModel);
                    return View(articleViewModel);
                }

                string? imagePath = article.Image;
                if (articleViewModel.ImageFile != null && articleViewModel.ImageFile.Length > 0)
                    imagePath = await _imageStorageService.SaveArticleImageAsync(articleViewModel.ImageFile);

                await _articleService.UpdateArticleAsync(
                    article.ArticleId,
                    articleViewModel.Title,
                    imagePath,
                    articleViewModel.Content,
                    articleViewModel.GenreId
                );
                return RedirectToAction("Index", "Blog");
            }

            await LoadGenres(articleViewModel);
            return View(articleViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            await _commentService.DeleteCommentsByArticleIdAsync(id);
            await _likeService.DeleteLikesByArticleIdAsync(id);
            await _articleService.DeleteArticleAsync(id);
            return RedirectToAction("Index", "Blog");
        }

        private async Task LoadGenres(ArticleViewModel articleViewModel)
        {
            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres.Select(GenreMapper.ToViewModel).ToList();
        }
    }
}
