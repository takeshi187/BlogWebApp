namespace BlogWebApp.ViewModels
{
    public class UserViewModel
    {

        public string Id { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public int CommentsCount { get; set; }

        public int LikesCount { get; set; }

        public IList<string> Roles { get; set; } = new List<string>();
    }
}
