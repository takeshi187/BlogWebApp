namespace BlogWebApp.ViewModels.ArticleViewModels
{
    public class ArticleListViewModel
    {
        public Guid ArticleListViewModelId { get; set; }
        public string Title { get; set; } = null!;
        public string? Image {  get; set; }
        public string ShortContent { get; set; } = null!;
        public string GenreName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
