namespace MyGymWorld.Web.ViewModels.Managers.Events
{
    using MyGymWorld.Common;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class EditEventInputModel
    {
        public EditEventInputModel()
        {
            this.EventTypes = new HashSet<string>();
        }

        public string Id { get; set; } = null!;

        [Required]
        [StringLength(ValidationalConstants.EventConstants.NameMaxLength, ErrorMessage = "The name mist be between 5 and 20 characters!",
            MinimumLength = ValidationalConstants.EventConstants.NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(ValidationalConstants.EventConstants.DescriptionMaxLength, ErrorMessage = "The description must be between 15 and 150 characters!",
            MinimumLength = ValidationalConstants.EventConstants.DescriptionMinLength)]
        public string Description { get; set; } = null!;

        public string StartDate { get; set; } = null!;

        public string EndDate { get; set; } = null!;

        [Required]
        public string EventType { get; set; } = null!;

        public IEnumerable<string> EventTypes { get; set; }

        public string GymId { get; set; } = null!;
    }
}
