namespace MyGymWorld.Web.ViewModels.Managers.Articles
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditArticleInputModel
    {
        public EditArticleInputModel()
        {
            this.CategoryIds = new HashSet<string>();
            this.CategoriesListItems = new HashSet<SelectListItem>();
        }

        [Required]
        [StringLength(ValidationalConstants.ArticleConstants.TitleMaxLength, ErrorMessage = "The title must be at least 10 characters and at most 50 characters long!",
            MinimumLength = ValidationalConstants.ArticleConstants.TitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(ValidationalConstants.ArticleConstants.ContentMaxLength, ErrorMessage = "The content must be at least 50 characters and at most 5000 characters long!",
            MinimumLength = ValidationalConstants.ArticleConstants.ContentMinLength)]
        public string Content { get; set; } = null!;

        public IEnumerable<string> CategoryIds { get; set; }

        public IEnumerable<SelectListItem> CategoriesListItems { get; set; }

        public string Id { get; set; } = null!;

        public string GymId { get; set; } = null!;
    }
}
