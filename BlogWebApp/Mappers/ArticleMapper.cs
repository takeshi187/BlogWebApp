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
                GenreName = article.Genre?.GenreName
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
