using BlogWebApp.Models;

namespace BlogWebApp.Services.GenreServices
{
    public interface IGenreService
    {
        Task<Genre> CreateGenreAsync(string genreName);
        Task<Genre?> GetGenreByIdAsync(Guid genreId);
        Task<IEnumerable<Genre?>> GetAllGenresAsync();
    }
}
