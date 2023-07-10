namespace MyGymWorld.Web.ViewModels.Administration.Roles
{
    using MyGymWorld.Common;
    using System.ComponentModel.DataAnnotations;

    public class EditRoleInputModel
    {
        public string? Id { get; set; } = null!;

        [Required]
        [StringLength(ValidationalConstants.ApplicationRoleConstants.NameMaxLength,
           MinimumLength = ValidationalConstants.ApplicationRoleConstants.NameMinLength)]
        public string Name { get; set; } = null!;
    }
}
