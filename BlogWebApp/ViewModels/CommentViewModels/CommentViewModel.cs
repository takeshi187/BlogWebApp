namespace BlogWebApp.ViewModels.CommentViewModels
{
    public class CommentViewModel
    {
        public Guid CommentId { get; set; }
        public string Content { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
