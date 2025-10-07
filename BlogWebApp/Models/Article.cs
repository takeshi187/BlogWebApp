namespace BlogWebApp.Models
{
    public class Article
    {
        private Guid _articleId;
        private string _title;
        private string? _image;
        private string _content;
        private int _likes;
        private List<Comment> _comments;
        private int _genreId;
        public DateTime _createdAt;
        public DateTime? _updatedAt;
        public Article()
        {
            
        }
    }
}
