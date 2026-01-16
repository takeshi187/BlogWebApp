using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Guid articleId, string userId, string content);
        Task<Comment?> GetCommentByIdAsync(Guid commentId);
        Task<bool> DeleteCommentsByArticleIdAsync(Guid articleId);
        Task<bool> DeleteCommentsByUserIdAsync(string userId);
    }
}
