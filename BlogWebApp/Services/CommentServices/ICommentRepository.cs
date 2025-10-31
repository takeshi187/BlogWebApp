using BlogWebApp.Models;

namespace BlogWebApp.Services.CommentServices
{
    public interface ICommentRepository
    {
        Task<IList<Comment>> GetByArticleIdAsync(Guid articleId);
        Task<Comment> GetByIdAsync(Guid commentId);
        Task<Comment> AddAsync(Comment comment);
        Task DeleteAsync(Comment comment);
        Task UpdateAsync(Comment comment);
    }
}
