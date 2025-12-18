using BlogWebApp.Mappers;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.CommentServices;
using BlogWebApp.Services.FileStorageServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.Services.LikeServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticleController> _logger;
        private readonly IGenreService _genreService;
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;
        private readonly IImageStorageService _imageStorageService;

        public ArticleController(
            IArticleService articleService,
            ILogger<ArticleController> logger,
            IGenreService genreService,
            ICommentService commentService,
            ILikeService likeService,
            IImageStorageService imageStorageService)
        {
            _articleService = articleService;
            _logger = logger;
            _genreService = genreService;
            _commentService = commentService;
            _likeService = likeService;
            _imageStorageService = imageStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var articleViewModel = new ArticleViewModel
            {
                Genres = genres.Select(GenreMapper.ToViewModel).ToList()
            };

            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                articleViewModel.Image = await _imageStorageService.SaveArticleImageAsync(articleViewModel.ImageFile);
                var article = ArticleMapper.ToEntity(articleViewModel);
                await _articleService.CreateArticleAsync(article);
                return RedirectToAction("Index", "Blog");
            }

            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres.Select(GenreMapper.ToViewModel).ToList();
            return View(articleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            return View(ArticleMapper.ToViewModel(article));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
                return NotFound();

            var articleViewModel = ArticleMapper.ToViewModel(article);
            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres.Select(GenreMapper.ToViewModel).ToList();
            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel articleViewModel)
        {
            if (ModelState.IsValid)
            {
                var article = await _articleService.GetArticleByIdAsync(articleViewModel.ArticleViewModelId);
                if (article == null)
                    return NotFound();

                ArticleMapper.MapToExistingEntity(articleViewModel, article);
                article.UpdatedAt = DateTime.UtcNow;
                await _articleService.UpdateArticleAsync(article);
                return RedirectToAction("Index", "Blog");
            }

            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres.Select(GenreMapper.ToViewModel).ToList();
            return View(articleViewModel);
        }

        [HttpGet]
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
    }
}
