using BlogWebApp.Models;

namespace BlogWebApp.Services.GenreServices
{
    public interface IGenreService
    {
        Task CreateGenreAsync(string genreName);
        Task<Genre?> GetGenreByIdAsync(Guid genreId);
        Task<IReadOnlyList<Genre>> GetAllGenresAsync();
    }
}
