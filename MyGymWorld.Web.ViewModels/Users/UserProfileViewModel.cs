namespace MyGymWorld.Web.ViewModels.Users
{
    public class UserProfileViewModel
    {
        public string Id { get; set; } = null!;

        public string UserName { get; set; } = null!; 
        
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePictureUri { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public int EventsCount { get; set; }

        public int ArticlesAcount { get; set; }

        public int LikesCount { get; set; }

        public int DislikesCount { get; set; }

        public int CommentsCount { get; set; }
    }
}