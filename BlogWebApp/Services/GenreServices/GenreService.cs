using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.GenreServices
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger<GenreService> _logger;

        public GenreService(IGenreRepository genreRepository, ILogger<GenreService> logger)
        {
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<bool> CreateGenreAsync(string genreName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(genreName))
                    throw new ArgumentException("Genre name cannot be empty.", nameof(genreName));

                var existing = await _genreRepository.GetByNameAsync(genreName);
                if (existing != null)
                    return false;

                var genre = new Genre(genreName);
                await _genreRepository.AddAsync(genre);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, $"Genre '{genreName}' already exists (race condition).");
                throw;
            }
        }

        public async Task<IReadOnlyList<Genre>> GetAllGenresAsync()
        {
            try
            {
                return await _genreRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching genres.");
                throw;
            }
        }
    }
}
