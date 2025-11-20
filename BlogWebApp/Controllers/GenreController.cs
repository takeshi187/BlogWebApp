using BlogWebApp.Mappers;
using BlogWebApp.Services.GenreServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogWebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenreService genreService, ILogger<GenreController> logger)
        {
            _genreService = genreService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new GenreViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreViewModel genreViewModel)
        {
            if(!ModelState.IsValid)
            {
                return View(genreViewModel);
            }

            var genre = GenreMapper.ToEntity(genreViewModel);
            await _genreService.CreateGenreAsync(genre.GenreName);

            return RedirectToAction("Create", "Article");
        }
    }
}
