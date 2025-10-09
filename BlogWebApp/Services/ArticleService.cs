using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public class ArticleService : IArticleService
    {
        public Task<Article> CreateArticleAsync(string title, string image, string content, int genreId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteArticleAsync(int articleId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Article>> GetAllArticlesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Article> GetArticleByIdAsync(int ArticleId)
        {
            throw new NotImplementedException();
        }

        public Task<Article> UpdateArticleAsync(string title, string image, string content, int genreId)
        {
            throw new NotImplementedException();
        }
    }
}
