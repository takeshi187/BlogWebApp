using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(Article article);
        Task<Article?> GetArticleByIdAsync(Guid ArticleId);
        Task<Article?> UpdateArticleAsync(Article article);
        Task<bool> DeleteArticleAsync(Guid articleId);
    }
}
