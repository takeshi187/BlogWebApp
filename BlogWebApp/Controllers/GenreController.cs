using BlogWebApp.Services.GenreServices;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("[controller]/[action]")]
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public IActionResult Create() => View(new GenreViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreViewModel genreViewModel)
        {
            try
            {
                await _genreService.CreateGenreAsync(genreViewModel.GenreName);
                return RedirectToAction("Create", "Article");
            }
            catch
            {
                ModelState.AddModelError(nameof(genreViewModel.GenreName), "Жанр с таким именем уже существует.");
                return View(genreViewModel);
            }
        }
    }
}
