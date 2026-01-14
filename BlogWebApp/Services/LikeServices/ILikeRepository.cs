using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeRepository
    {
        Task<Like> AddAsync(Like like);
        Task<Like?> GetByIdAsync(Guid likeId);
        Task<IEnumerable<Like?>> GetByArticleIdAsync(Guid articleId);
        Task<IEnumerable<Like?>> GetByUserIdAsync(string userId);
        Task<Like> ExistAsync(Guid articleId, string userId);
        Task<bool> DeleteRangeAsync(IEnumerable<Like> likes);
        Task<bool> DeleteAsync(Like like);
    }
}
