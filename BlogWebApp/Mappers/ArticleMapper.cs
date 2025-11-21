using BlogWebApp.Models;
using BlogWebApp.ViewModels.ArticleViewModels;
using BlogWebApp.ViewModels.CommentViewModels;

namespace BlogWebApp.Mappers
{
    public static class ArticleMapper
    {
        public static Article ToEntity(ArticleCreateViewModel articleViewModel)
        {
            return new Article
                (articleViewModel.Title, articleViewModel.Image, articleViewModel.Content, articleViewModel.GenreId);
        }

        public static void MapToExistingEntity(ArticleCreateViewModel articleViewModel, Article article)
        {
            article.Title = articleViewModel.Title;
            article.Content = articleViewModel.Content;
            article.Image = articleViewModel.Image;
            article.GenreId = articleViewModel.GenreId;
        }

        public static ArticleListViewModel ToListViewModel(Article article)
        {
            return new ArticleListViewModel
            {
                ArticleListViewModelId = article.ArticleId,
                Title = article.Title,
                ShortContent = article.Content.Length > 300
                ? article.Content.Substring(0, 300) + "..."
                : article.Content,
                Image = article.Image,
                GenreName = article.Genre.GenreName,
                CreatedAt = article.CreatedAt,
                LikesCount = article.Likes?.Count ?? 0,
                CommentsCount = article.Comments?.Count ?? 0
            };
        }

        public static ArticleDetailsViewModel ToDetailsViewModel(Article article)
        {
            return new ArticleDetailsViewModel
            {
                ArticleDetailsViewModelId = article.ArticleId,
                Title = article.Title,
                Content = article.Content,
                Image = article.Image,
                GenreName = article.Genre.GenreName,
                CreatedAt = article.CreatedAt,
                LikesCount = article.Likes?.Count ?? 0,
                CommentsCound = article.Comments?.Count ?? 0,
                Comments = article.Comments?
                    .Select(CommentMapper.ToViewModel)
                    .ToList() ?? new List<CommentViewModel>()
            };
        }
    }
}
