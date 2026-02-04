using BlogWebApp.Db;
using BlogWebApp.Mappers;
using BlogWebApp.Models;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IArticleService _articleService;

        public BlogController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? query)
        {
            var articles = await _articleService.GetAllArticlesAsync();
            if (articles == null || !articles.Any())
                return View(new List<ArticleViewModel>());

            IEnumerable<Article> filteredArticles = articles;

            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim();

                filteredArticles = filteredArticles.Where(a =>
                    a.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    a.Content.Contains(query, StringComparison.OrdinalIgnoreCase));
            }

            var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
            var articleViewModel = filteredArticles.Select(a => ArticleMapper.ToViewModel(a, userId)).ToList();

            ViewBag.Query = query;

            return View(articleViewModel);
        }      
    }
}
