namespace MyGymWorld.Web.ViewModels.Comments
{
    using MyGymWorld.Common;
    using System.ComponentModel.DataAnnotations;

    public class CreateCommentInputModel
    {
        [Required]
        [StringLength(ValidationalConstants.CommentConstants.ContentMaxLength, ErrorMessage = "Content must be at least 5 symbols long and at most 500 symbols long!",
            MinimumLength = ValidationalConstants.CommentConstants.ContentMinLength)]
        public string Content { get; set; } = null!;

        public string UserId { get; set; } = null!;

        public string GymId { get; set; } = null!;
    }
}
