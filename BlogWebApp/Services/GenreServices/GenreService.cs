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

        public async Task CreateGenreAsync(string genreName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(genreName))
                    throw new ArgumentException("Genre name cannot be empty.", nameof(genreName));

                var genre = new Genre(genreName);
                await _genreRepository.AddAsync(genre);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex, $"Genre '{genreName}' already exists (race condition).");
                throw new InvalidOperationException($"Genre '{genreName}' already exists.", ex);
            }
        }

        public async Task<Genre?> GetGenreByIdAsync(Guid genreId)
        {

            if (genreId == Guid.Empty)
                throw new ArgumentException("genreId cannot be empty.", nameof(genreId));

            var genre = await _genreRepository.GetByIdAsync(genreId);
            if (genre == null)
                throw new InvalidOperationException($"Genre with id: {genreId} not found.");

            return genre;
        }

        public async Task<IReadOnlyList<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }
    }
}
