using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(Comment comment);
        Task<Comment?> GetCommentByIdAsync(Guid commentId);
        Task<IEnumerable<Comment?>> GetCommentsByArticleIdAsync(Guid articleId);
        Task<Comment?> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentsByArticleIdAsync(Guid articleId);
        Task<bool> DeleteCommentAsync(Guid commentId);
    }
}
