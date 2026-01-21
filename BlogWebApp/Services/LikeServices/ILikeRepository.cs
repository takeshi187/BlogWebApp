using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeRepository
    {
        Task AddAsync(Like like);
        Task<Like?> GetByIdAsync(Guid likeId);
        Task<IEnumerable<Like?>> GetByArticleIdAsync(Guid articleId);
        Task<IEnumerable<Like?>> GetByUserIdAsync(string userId);
        Task<Like> ExistAsync(Guid articleId, string userId);
        Task DeleteRangeAsync(IEnumerable<Like> likes);
        Task DeleteAsync(Like like);
    }
}
