using BlogWebApp.Models;
using BlogWebApp.ViewModels;

namespace BlogWebApp.Mappers
{
    public static class ArticleMapper
    {
        public static ArticleViewModel ToViewModel(Article article)
        {
            return new ArticleViewModel
            {
                ArticleViewModelId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                Image = article.Image,
                GenreId = article.GenreId,
                GenreName = article.Genre.GenreName,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
                LikesCount = article.Likes?.Count ?? 0,
                CommentsCount = article.Comments?.Count ?? 0,
                Comments = article.Comments.Select(c => new CommentViewModel
                {
                    CommentId = c.CommentId,
                    Content = c.Content,
                    ArticleId = c.ArticleId,
                    UserName = c.User.UserName,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList()
            };
        }

        public static Article ToEntity(ArticleViewModel articleViewModel)
        {
            return new Article
                (articleViewModel.Title, articleViewModel.Image, articleViewModel.Content, articleViewModel.GenreId);
        }

        public static void MapToExistingEntity(ArticleViewModel articleViewModel, Article article)
        {
            article.Title = articleViewModel.Title;
            article.Content = articleViewModel.Content;
            article.Image = articleViewModel.Image;
            article.GenreId = articleViewModel.GenreId;
        }
    }
}
