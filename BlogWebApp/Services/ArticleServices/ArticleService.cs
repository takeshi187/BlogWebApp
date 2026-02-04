using BlogWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Services.ArticleServices
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<ArticleService> _logger;

        public ArticleService(IArticleRepository articleRepository, ILogger<ArticleService> logger)
        {
            _articleRepository = articleRepository;
            _logger = logger;
        }

        public async Task CreateArticleAsync(string title, string? image, string content, Guid genreId)
        {
            try
            {
                var article = new Article(title, image, content, genreId);
                await _articleRepository.AddAsync(article);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding article with title: {title}");
                throw new InvalidOperationException("Failed to add article to database.", ex);
            }
        }

        public async Task<Article?> GetArticleByIdAsync(Guid articleId)
        {

            if (articleId == Guid.Empty)
                throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

            var article = await _articleRepository.GetByIdAsync(articleId);
            if (article == null)
                throw new InvalidOperationException($"Article with id: {articleId} not found.");

            return article;
        }

        public async Task<IReadOnlyList<Article>> GetAllArticlesAsync()
        {
            try
            {
                return await _articleRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching articles.");
                throw;
            }
        }

        public async Task UpdateArticleAsync(Guid articleId, string title, string? image, string content, Guid genreId)
        {
            try
            {
                var existingArticle = await _articleRepository.GetByIdAsync(articleId);
                if (existingArticle == null)
                    throw new InvalidOperationException($"Article with id: {articleId} not found.");

                existingArticle.Update(title, image, content, genreId);
                await _articleRepository.UpdateAsync(existingArticle);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while updating article: {articleId}");
                throw new InvalidOperationException("Failed to update article.", ex);
            }
        }

        public async Task DeleteArticleAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var article = await _articleRepository.GetByIdAsync(articleId);

                if (article == null)
                    throw new InvalidOperationException($"Article with id: {articleId} not found.");

                await _articleRepository.DeleteAsync(article);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting article: {articleId}");
                throw new InvalidOperationException("Failed to delete article.", ex);
            }
        }
    }
}
