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

        public async Task<Comment> AddAsync(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
            return comment;
        }

        public async Task DeleteAsync(Comment comment)
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
        }

        public async Task<IList<Comment>> GetByArticleIdAsync(Guid articleId)
        {
            return await _db.Comments.Where(c => c.ArticleId == articleId).ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(Guid commentId)
        {
            return await _db.Comments.FindAsync(commentId);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _db.Comments.Update(comment);
            await _db.SaveChangesAsync();
        }
    }
}
