using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public interface IArticleRepository
    {
        Task<Article> AddAsync(Article article);
        Task<Article?> GetByIdAsync(Guid articleId);
        Task<bool> UpdateAsync(Article article);
        Task<bool> DeleteAsync(Article article);
    }
}
