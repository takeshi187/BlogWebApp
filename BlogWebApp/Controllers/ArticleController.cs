using BlogWebApp.Mappers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Formats.Asn1;

namespace BlogWebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ILogger<ArticleController> _logger;
        private readonly IGenreService _genreService;

        public ArticleController(IArticleService articleService, ILogger<ArticleController> logger, IGenreService genreService)
        {
            _articleService = articleService;
            _logger = logger;
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            if (!articles.Any())
                return View(new List<ArticleViewModel>());
            var articleViewModel = articles.Select(ArticleMapper.ToViewModel).ToList();
            return View(articleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var genres = await _genreService.GetAllGenresAsync();

            var articleViewModel = new ArticleViewModel
            {
                Genres = genres?.ToList() ?? new List<Genre?>()
            };

            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel articleViewModel)
        {
            if (!ModelState.IsValid)
            {
                var genres = await _genreService.GetAllGenresAsync();
                articleViewModel.Genres = genres?.ToList() ?? new List<Genre?>();
                return View(articleViewModel);
            }

            var article = ArticleMapper.ToEntity(articleViewModel);
            await _articleService.CreateArticleAsync(article);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            if (article == null) 
                return NotFound();
            return View(ArticleMapper.ToViewModel(article));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            if (article == null) 
                return NotFound();
            var articleViewModel = ArticleMapper.ToViewModel(article);
            var genres = await _genreService.GetAllGenresAsync();
            articleViewModel.Genres = genres?.ToList() ?? new List<Genre?>();
            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ArticleViewModel articleViewModel)
        {
            if(!ModelState.IsValid)
            {
                var genres = await _genreService.GetAllGenresAsync();
                articleViewModel.Genres = genres?.ToList() ?? new List<Genre?>();
                return View(articleViewModel);
            }
            var article = await _articleService.GetArticleByIdAsync(articleViewModel.ArticleViewModelId);
            if (article == null) 
                return NotFound();
            ArticleMapper.MapToExistingEntity(articleViewModel, article);
            await _articleService.UpdateArticleAsync(article);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid articleId)
        {
            await _articleService.DeleteArticleAsync(articleId);
            return RedirectToAction(nameof(Index));
        }
    }
}
