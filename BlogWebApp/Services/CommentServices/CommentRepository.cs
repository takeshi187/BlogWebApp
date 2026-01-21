using BlogWebApp.Db;
using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.CommentServices
{
    public class CommentRepository : ICommentRepository
    {
        private readonly BlogWebAppDbContext _db;

        public CommentRepository(BlogWebAppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid commentId)
        {
            return await _db.Comments.FindAsync(commentId);
        }

        public async Task<IEnumerable<Comment?>> GetByArticleIdAsync(Guid articleId)
        {
            return await _db.Comments.Where(c => c.ArticleId == articleId).ToListAsync();
        }

        public async Task<IEnumerable<Comment?>> GetByUserIdAsync(string userId)
        {
            return await _db.Comments.Where(c => c.UserId == userId).ToListAsync();
        }     

        public async Task DeleteRangeAsync(IEnumerable<Comment> comments)
        {
            _db.Comments.RemoveRange(comments);
            await _db.SaveChangesAsync();
        }
    }
}
