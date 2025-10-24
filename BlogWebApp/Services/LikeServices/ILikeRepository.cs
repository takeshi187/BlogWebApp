using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeRepository
    {
        Task<Like> GetByIdAsync(Guid likeId);
        Task<IList<Like>> GetByArticleIdAsync(Guid articleId);
        Task AddAsync(Like like);
        Task DeleteAsync(Like like);
        Task<bool> ExistAsync(Guid articleId, string userId);
    }
}
