using BlogWebApp.Models;

namespace BlogWebApp.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<Article> CreateArticleAsync(string title, string image, string content, int genreId)
        {
            var article = new Article(title, image, content, 0, genreId, DateOnly.FromDateTime(DateTime.Now), null);
            return await _articleRepository.AddAsync(article);
        }

        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) return false;
            await _articleRepository.DeleteAsync(articleId);
            return true;
        }

        public Task<IList<Article>> GetAllArticlesAsync()
        {
            return _articleRepository.GetAllAsync();
        }

        public async Task<Article> GetArticleByIdAsync(int articleId)
        {
            return await _articleRepository.GetByIdAsync(articleId);
        }

        public async Task<Article> UpdateArticleAsync(int articleId, string title, string image, string content, int genreId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) return null;
            article.Title = title;
            article.Image = image;
            article.Content = content;
            article.GenreId = genreId;
            article.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
            return await _articleRepository.UpdateAsync(article);
        }
    }
}
