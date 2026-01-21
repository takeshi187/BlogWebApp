using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid commentId);
        Task<IEnumerable<Comment?>> GetByArticleIdAsync(Guid articleId);
        Task<IEnumerable<Comment?>> GetByUserIdAsync(string userId);
        Task DeleteRangeAsync(IEnumerable<Comment> comments);
    }
}
