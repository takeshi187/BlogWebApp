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
        
        public async Task<Genre> AddAsync(Genre genre)
        {
            await _db.Genres.AddAsync(genre);
            await _db.SaveChangesAsync();
            return genre;
        }

        public async Task<Genre?> GetByIdAsync(Guid genreId)
        {
            return await _db.Genres.FindAsync(genreId);
        }

        public async Task<IEnumerable<Genre?>> GetAllAsync()
        {
            return await _db.Genres.ToListAsync();
        }
    }
}
