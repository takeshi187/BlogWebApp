using BlogWebApp.ViewModels.CommentViewModels;

namespace BlogWebApp.ViewModels.ArticleViewModels
{
    public class ArticleDetailsViewModel
    {
        public Guid ArticleDetailsViewModelId { get; set; }
        public string Title { get; set; } = null!;
        public string? Image {  get; set; }
        public string Content { get; set; } = null!;
        public string GenreName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCound { get; set; }
        public List<CommentViewModel> Comments { get; set; } = new();
    }
}
