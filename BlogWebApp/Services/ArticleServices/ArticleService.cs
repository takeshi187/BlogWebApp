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

        public async Task<Article> CreateArticleAsync(Article article)
        {
            try
            {
                if (article == null)
                    throw new ArgumentException("Article cannot be empty.", nameof(article));
                if (string.IsNullOrWhiteSpace(article.Title))
                    throw new ArgumentException("Title cannot be empty.", nameof(article.Title));
                if (string.IsNullOrWhiteSpace(article.Content))
                    throw new ArgumentException("Content cannot be empty.", nameof(article.Content));
                if (article.GenreId == Guid.Empty)
                    throw new ArgumentException("GenreId cannot be empty.", nameof(article.GenreId));

                await _articleRepository.AddAsync(article);
                return article;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid article data for creation.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding article {article.Title}");
                throw new InvalidOperationException("Failed to add article to database.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while adding article {article.Title}");
                throw;
            }
        }

        public async Task<Article?> GetArticleByIdAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty) 
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var article = await _articleRepository.GetByIdAsync(articleId);
                if (article == null) 
                    throw new InvalidOperationException($"Article with id: {articleId} not found.");

                return article;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Article not found: {articleId}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while searching article with id: {articleId}");
                throw;
            }
        }

        public async Task<IEnumerable<Article?>> GetAllArticlesAsync()
        {
            try
            {
                return await _articleRepository.GetAllAsync();

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while searching articles.");
                throw;
            }
        }

        public async Task<Article?> UpdateArticleAsync(Article article)
        {
            try
            {
                if (article == null)
                    throw new ArgumentException("Article cannot be empty.", nameof(article));
                if (string.IsNullOrWhiteSpace(article.Title))
                    throw new ArgumentException("Title cannot be empty.", nameof(article.Title));
                if (string.IsNullOrWhiteSpace(article.Content))
                    throw new ArgumentException("Content cannot be empty.", nameof(article.Content));
                if (article.GenreId == Guid.Empty)
                    throw new ArgumentException("GenreId cannot be empty.", nameof(article.GenreId));

                var existingArticle = await _articleRepository.GetByIdAsync(article.ArticleId);
                if (existingArticle == null) 
                    throw new InvalidOperationException($"Article with id: {article.ArticleId} not found.");

                existingArticle.Title = article.Title;
                existingArticle.Image = article.Image;
                existingArticle.Content = article.Content;
                existingArticle.GenreId = article.GenreId;
                existingArticle.UpdatedAt = DateTime.UtcNow;
                await _articleRepository.UpdateAsync(existingArticle);
                return existingArticle;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid article data for updating.");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while updating article: {article.ArticleId}");
                throw new InvalidOperationException("Failed to update article.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while updating article: {article.ArticleId}");
                throw;
            }
        }

        public async Task<bool> DeleteArticleAsync(Guid articleId)
        {
            try
            {
                if (articleId == Guid.Empty)
                    throw new ArgumentException("ArticleId cannot be empty.", nameof(articleId));

                var article = await _articleRepository.GetByIdAsync(articleId);
                if (article == null) 
                    throw new InvalidOperationException($"Article with id: {articleId} not found.");

                return await _articleRepository.DeleteAsync(article);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, $"Article not found: {articleId}");
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting article: {articleId}");
                throw new InvalidOperationException("Failed to delete article.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while deleting article: {articleId}");
                throw;
            }
        }
    }
}
