using BlogWebApp.Models;

namespace BlogWebApp.Services.GenreServices
{
    public interface IGenreRepository
    {
        Task<Genre> AddAsync(Genre genre);
        Task<Genre?> GetByIdAsync(Guid genreId);
        Task<IEnumerable<Genre?>> GetAllAsync();
    }
}
