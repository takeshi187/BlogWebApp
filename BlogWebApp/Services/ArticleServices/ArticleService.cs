using BlogWebApp.Models;

namespace BlogWebApp.Services.ArticleServices
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<Article> CreateArticleAsync(Article article)
        {
            if(string.IsNullOrEmpty(article.Title))
                throw new ArgumentException("Title cannot be empty", nameof(article.Title));
            if(string.IsNullOrEmpty(article.Content))
                throw new ArgumentException("Content cannot be empty", nameof(article.Content));
            if (string.IsNullOrEmpty(article.GenreId.ToString()))
                throw new ArgumentException("GenreId cannot be empty", nameof(article.GenreId));

            await _articleRepository.AddAsync(article);
            return article;
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) throw new InvalidOperationException($"Article with id {articleId} not found."); 
            await _articleRepository.DeleteAsync(article);
        }

        public Task<IList<Article>> GetAllArticlesAsync()
        {
            var listOfArticles = _articleRepository.GetAllAsync();
            if (listOfArticles == null) throw new InvalidOperationException($"No one articles not found.");
            return listOfArticles;
        }

        public async Task<Article> GetArticleByIdAsync(Guid articleId)
        {
            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null) throw new InvalidOperationException($"Article with id {articleId} not found.");
            return article;
        }
        public async Task<Article> UpdateArticleAsync(Article article)
        {
            if (string.IsNullOrEmpty(article.Title))
                throw new ArgumentNullException("Title cannot be empty", nameof(article.Title));
            if (string.IsNullOrEmpty(article.Content))
                throw new ArgumentNullException("Content cannot be empty", nameof(article.Content));
            if (string.IsNullOrEmpty(article.GenreId.ToString()))
                throw new ArgumentNullException("GenreId cannot be empty", nameof(article.GenreId));

            var existingArticle = await _articleRepository.GetByIdAsync(article.ArticleId);
            if (existingArticle == null) throw new InvalidOperationException($"Article with id {article.ArticleId} not found.");
            existingArticle.Title = article.Title;
            existingArticle.Image = article.Image;
            existingArticle.Content = article.Content;
            existingArticle.GenreId = article.GenreId;
            existingArticle.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            await _articleRepository.UpdateAsync(existingArticle);
            return existingArticle;
        }
    }
}
