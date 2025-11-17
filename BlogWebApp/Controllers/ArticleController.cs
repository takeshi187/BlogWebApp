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
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var genres = await _genreService.GetAllGenresAsync();

            var articleViewModel = new ArticleViewModel();

            ViewBag.Genres = new SelectList(genres, "Id", "Name");

            return View(articleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArticleViewModel articleViewModel)
        {
            if(!ModelState.IsValid)
            {
                var genres = await _genreService.GetAllGenresAsync();
                ViewBag.Genres = new SelectList(genres, "Id", "Name");
                var article = new Article
                {
                    Title = articleViewModel.Title,
                    Image = articleViewModel.Image,
                    Content = articleViewModel.Content,
                    GenreId = articleViewModel.GenreId
                };
                
                await _articleService.CreateArticleAsync(article);
                return View(articleViewModel);
            }
          
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid articleId)
        {
            var article = await _articleService.GetArticleByIdAsync(articleId);
            if (article == null) return NotFound();

            return View(article);
        }
    }
}
