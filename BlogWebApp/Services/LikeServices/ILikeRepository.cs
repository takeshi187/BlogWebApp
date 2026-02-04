using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeRepository
    {
        Task AddAsync(Like like);
        Task<Like?> GetByIdAsync(Guid likeId);
        Task<IReadOnlyList<Like>> GetByArticleIdAsync(Guid articleId);
        Task<IReadOnlyList<Like>> GetByUserIdAsync(string userId);
        Task<Like> GetByArticleIdAndUserIdAsync(Guid articleId, string userId);
        Task DeleteRangeAsync(IReadOnlyCollection<Like> likes);
        Task DeleteAsync(Like like);
    }
}
