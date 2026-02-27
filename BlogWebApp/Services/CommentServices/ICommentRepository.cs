using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid commentId);
        Task<IReadOnlyList<Comment>> GetByArticleIdAsync(Guid articleId);
        Task<IReadOnlyList<Comment>> GetByUserIdAsync(string userId);
        Task DeleteRangeAsync(IReadOnlyCollection<Comment> comments);
        Task DeleteAsync(Comment comment);
    }
}
