using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentRepository
    {
        Task<Comment> AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid commentId);
        Task<IEnumerable<Comment?>> GetByArticleIdAsync(Guid articleId);
        Task<bool> UpdateAsync(Comment comment);
        Task<bool> DeleteAsync(Comment comment);
    }
}
