using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentService
    {
        Task<Comment> AddCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(Guid commentId);
        Task<bool> UpdateCommentAsync(Guid commentId, string newContent);
        Task<IList<Comment>> GetCommentsByArticleAsync(Guid articleId);
        Task<Comment> GetCommentByIdAsync(Guid commentId);
    }
}
