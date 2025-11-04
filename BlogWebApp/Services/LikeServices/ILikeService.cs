using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeService
    {
        Task<bool> AddLikeAsync(Guid articleId, string userId);
        Task<Like?> GetLikeByIdAsync(Guid likeId);
        Task<IEnumerable<Like?>>GetLikesForArticleAsync(Guid articleId);
        Task<bool> DeleteLikeAsync(Guid articleId, string userId);
    }
}
