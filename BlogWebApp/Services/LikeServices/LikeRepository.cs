using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.LikeServices
{
    public class LikeRepository : ILikeRepository
    {
        private readonly BlogWebAppDbContext _db;

        public LikeRepository(BlogWebAppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Like like)
        {
            await _db.Likes.AddAsync(like);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Like like)
        {
            _db.Likes.Remove(like);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistAsync(Guid articleId, string userId)
        {
            return await _db.Likes.AnyAsync(l => l.ArticleId == articleId && l.UserId == userId);
        }

        public async Task<IList<Like>> GetByArticleIdAsync(Guid articleId)
        {
            return await _db.Likes.Where(l => l.ArticleId == articleId).ToListAsync();
        }

        public async Task<Like> GetByIdAsync(Guid likeId)
        {
            return await _db.Likes.FindAsync(likeId);
        }
    }
}
