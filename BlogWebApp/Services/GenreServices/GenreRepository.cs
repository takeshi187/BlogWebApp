using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.GenreServices
{
    public class GenreRepository : IGenreRepository
    {
        private readonly BlogWebAppDbContext _db;

        public GenreRepository(BlogWebAppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Genre genre)
        {
            await _db.Genres.AddAsync(genre);
            await _db.SaveChangesAsync();
        }

        public async Task<Genre?> GetByIdAsync(Guid genreId)
        {
            return await _db.Genres.FirstOrDefaultAsync(g => g.GenreId == genreId);
        }

        public async Task<Genre?> GetByNameAsync(string genreName)
        {
            return await _db.Genres.FirstOrDefaultAsync(g => g.GenreName == genreName);
        }

        public async Task<IReadOnlyList<Genre>> GetAllAsync()
        {
            return await _db.Genres.ToListAsync();
        }
    }
}
