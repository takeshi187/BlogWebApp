using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentService
    {
        Task CreateCommentAsync(Guid articleId, string userId, string content);
        Task<Comment?> GetCommentByIdAsync(Guid commentId);
        Task DeleteCommentsByArticleIdAsync(Guid articleId);
        Task DeleteCommentsByUserIdAsync(string userId);
    }
}
