using BlogWebApp.Mappers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IGenreService _genreService;

        public BlogController(IArticleService articleService, IGenreService genreService)
        {
            _articleService = articleService;
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? query, Guid? genreId)
        {
            var articles = await _articleService.GetAllArticlesAsync();
            IEnumerable<Article> filteredArticles = articles;

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();

                filteredArticles = filteredArticles.Where(a =>
                    a.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    a.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            if(genreId.HasValue && genreId != Guid.Empty)
            {
                filteredArticles = filteredArticles.Where(a => a.GenreId == genreId.Value);
                ViewBag.SelectedGenreId = genreId.Value;    
            }

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            var articleViewModel = filteredArticles.Select(a => ArticleMapper.ToViewModel(a, userId)).ToList();

            ViewBag.Genres = await _genreService.GetAllGenresAsync();
            ViewBag.Query = query;

            return View(articleViewModel);
        }
    }
}
