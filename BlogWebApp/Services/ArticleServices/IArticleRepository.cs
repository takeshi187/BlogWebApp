using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public interface IArticleRepository
    {
        Task AddAsync(Article article);
        Task<Article?> GetByIdAsync(Guid articleId);
        Task<IReadOnlyList<Article>> GetAllAsync();
        Task UpdateAsync(Article article);
        Task DeleteAsync(Article article);
    }
}
