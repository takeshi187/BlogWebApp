using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<Comment?> GetCommentByIdAsync(Guid commentId);
        Task<IEnumerable<Comment?>> GetCommentsByArticleAsync(Guid articleId);
        Task<Comment?> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid commentId);
        
    }
}
