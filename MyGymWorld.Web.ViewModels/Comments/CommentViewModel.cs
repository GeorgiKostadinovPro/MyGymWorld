namespace MyGymWorld.Web.ViewModels.Comments
{
    public class CommentViewModel
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = null!; 
        
        public string GymId { get; set; } = null!;

        public string? ParentId { get; set; }

        public string UserId { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string? AuthorProfilePictureUri { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
