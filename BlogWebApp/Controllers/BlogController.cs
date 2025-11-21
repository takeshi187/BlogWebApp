using BlogWebApp.Mappers;
using BlogWebApp.Services.ArticleServices;
using BlogWebApp.ViewModels.ArticleViewModels;
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
                return View(new List<ArticleCreateViewModel>());
            var articleViewModel = articles.Select(ArticleMapper.ToListViewModel).ToList();
            return View(articleViewModel);
        }
    }
}
