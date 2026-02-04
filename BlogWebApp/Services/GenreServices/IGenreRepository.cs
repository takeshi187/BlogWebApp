using BlogWebApp.Models;

namespace BlogWebApp.Services.GenreServices
{
    public interface IGenreRepository
    {
        Task AddAsync(Genre genre);
        Task<Genre?> GetByIdAsync(Guid genreId);
        Task<Genre?> GetByNameAsync(string genreName);
        Task<IReadOnlyList<Genre>> GetAllAsync();
    }
}
