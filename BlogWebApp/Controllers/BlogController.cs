using BlogWebApp.Mappers;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Index()
        {
            var articles = await _articleService.GetAllArticlesAsync();
            if (!articles.Any())
                return View(new List<ArticleViewModel>());
            var articleViewModel = articles.Select(ArticleMapper.ToViewModel).ToList();
            return View(articleViewModel);
        }
    }
}
