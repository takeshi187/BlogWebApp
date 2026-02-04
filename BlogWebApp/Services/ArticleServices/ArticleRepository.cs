using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.ArticleServices
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly BlogWebAppDbContext _db;

        public ArticleRepository(BlogWebAppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Article article)
        {
            await _db.Articles.AddAsync(article);
            await _db.SaveChangesAsync();
        }

        public async Task<Article?> GetByIdAsync(Guid articleId)
        {
            return await _db.Articles
                .Include(a => a.Genre)
                .Include(a => a.Likes)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.ArticleId == articleId);
        }

        public async Task<IReadOnlyList<Article>> GetAllAsync()
        {
            return await _db.Articles
                .Include(a => a.Genre)
                .Include(a => a.Likes)
                .Include(a => a.Comments)
                    .ThenInclude(c => c.User)
                .ToListAsync();
        }

        public async Task UpdateAsync(Article article)
        {
            _db.Articles.Update(article);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Article article)
        {
            _db.Articles.Remove(article);
            await _db.SaveChangesAsync();
        }
    }
}
