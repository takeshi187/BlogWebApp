using BlogWebApp.Models;

namespace BlogWebApp.Services.GenreServices
{
    public interface IGenreService
    {
        Task<bool> CreateGenreAsync(string genreName);
        Task<IReadOnlyList<Genre>> GetAllGenresAsync();
    }
}
