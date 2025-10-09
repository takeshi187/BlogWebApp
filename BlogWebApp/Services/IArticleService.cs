using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(string title, string image, string content, int genreId);
        Task<Article> GetArticleByIdAsync(int ArticleId);
        Task<IList<Article>> GetAllArticlesAsync();
        Task<Article> UpdateArticleAsync(string title, string image, string content, int genreId);
        Task<bool> DeleteArticleAsync(int articleId);
    }
}
