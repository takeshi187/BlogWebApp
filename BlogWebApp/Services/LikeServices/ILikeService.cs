using BlogWebApp.Models;

namespace BlogWebApp.Services.LikeServices
{
    public interface ILikeService
    {
        Task ToggleLikeAsync(Guid articleId, string userId);
        Task<Like?> GetLikeByIdAsync(Guid likeId);
        Task<bool> DeleteLikesByArticleIdAsync(Guid articleId);
        Task<bool> DeleteLikesByUserIdAsync(string userId);
    }
}
