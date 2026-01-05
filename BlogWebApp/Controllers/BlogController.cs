using BlogWebApp.Db;
using BlogWebApp.Mappers;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly BlogWebAppDbContext _db;

        public BlogController(IArticleService articleService, BlogWebAppDbContext db)
        {
            _articleService = articleService;
            _db = db;
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
        public async Task<IActionResult> Search(string query)
        {
            var articlesQuery = _db.Articles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                articlesQuery = articlesQuery
                    .Where(a => a.Title.ToLower().Contains(query.ToLower()) || a.Content.ToLower().Contains(query.ToLower()));
            }

            var articles = await articlesQuery
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ArticleViewModel
                {
                    ArticleViewModelId = a.ArticleId,
                    Title = a.Title,
                    Content = a.Content,
                    Image = a.Image,
                    GenreName = a.Genre.GenreName,
                    CreatedAt = a.CreatedAt,
                    LikesCount = a.Likes.Count,
                    CommentsCount = a.Comments.Count
                })
                .ToListAsync();

            return View("Search", articles);
        }
    }
}
