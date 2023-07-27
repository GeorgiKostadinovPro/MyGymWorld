namespace MyGymWorld.Web.ViewModels.Managers.Articles
{
    using MyGymWorld.Common;
    using MyGymWorld.Data.Models;
    using System.ComponentModel.DataAnnotations;

    public class CreateArticleInputModel
    {
        public CreateArticleInputModel()
        {
            this.Categories = new HashSet<Category>();
        }

        [Required]
        [StringLength(ValidationalConstants.ArticleConstants.TitleMaxLength, ErrorMessage = "The title must be at least 10 characters and at most 50 characters long!",
            MinimumLength = ValidationalConstants.ArticleConstants.TitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(ValidationalConstants.ArticleConstants.ContentMaxLength, ErrorMessage = "The content must be at least 50 characters and at most 5000 characters long!",
            MinimumLength = ValidationalConstants.ArticleConstants.ContentMinLength)]
        public string Content { get; set; } = null!;

        [Required]
        public string CategoryId { get; set; } = null!;

        public IEnumerable<Category> Categories { get; set; }

        public string GymId { get; set; } = null!;
    }
}
