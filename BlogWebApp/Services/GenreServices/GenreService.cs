using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.GenreServices
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger _logger;

        public GenreService(IGenreRepository genreRepository, ILogger logger)
        {
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<Genre> CreateGenreAsync(string genreName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(genreName))
                    throw new ArgumentException("Genre name cannot be empty.", nameof(genreName));

                var existingGenre = (await _genreRepository.GetAllAsync())
                    .FirstOrDefault(g => g.GenreName.Equals(genreName, StringComparison.OrdinalIgnoreCase));

                if (existingGenre != null)
                    throw new InvalidOperationException($"Genre with name: {genreName} is already exist.");

                var genre = new Genre(genreName);

                await _genreRepository.AddAsync(genre);
                return genre;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid genre data for adding.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding genre: {genreName}");
                throw new InvalidOperationException("Failed to add genre to database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding genre: {genreName}");
                throw;
            }
        }

        public async Task<Genre?> GetGenreByIdAsync(Guid genreId)
        {
            try
            {
                if (genreId == Guid.Empty)
                    throw new ArgumentException("genreId cannot be empty.", nameof(genreId));
                var genre = await _genreRepository.GetByIdAsync(genreId);
                if (genre == null) throw new InvalidOperationException($"Genre with id: {genreId} not found.");

                return genre;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Genre with id: {genreId} not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching genre with id: {genreId}");
                throw;
            }
        }

        public async Task<IEnumerable<Genre?>> GetAllGenresAsync()
        {
            try
            {
                var genres = await _genreRepository.GetAllAsync();

                if (genres == null || !genres.Any())
                    throw new InvalidOperationException($"no one genre not found.");

                return genres;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"No one genre not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching genres");
                throw;
            }
        }

    }
}
