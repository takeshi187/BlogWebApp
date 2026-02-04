using BlogWebApp.Models;
using BlogWebApp.ViewModels;

namespace BlogWebApp.Mappers
{
    public static class ArticleMapper
    {
        public static ArticleViewModel ToViewModel(Article article, string userId = null)
        {
            return new ArticleViewModel
            {
                ArticleViewModelId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                Image = article.Image,
                GenreId = article.GenreId,
                GenreName = article.Genre?.GenreName,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                LikesCount = article.Likes?.Count ?? 0,
                CommentsCount = article.Comments?.Count ?? 0,
                Comments = article.Comments?.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    ArticleId = c.ArticleId,
                    UserName = c.User?.UserName ?? "Неизвестный",
                    CreatedAt = c.CreatedAt,
                }).ToList() ?? new List<CommentViewModel>(),
                UserHasLiked = userId != null && article.Likes != null && article.Likes.Any(l => l.UserId.ToString() == userId)
            };
        }
    }
}
