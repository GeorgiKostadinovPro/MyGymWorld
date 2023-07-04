namespace MyGymWorld.Web.ViewModels.Users
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    public class EditUserInputModel
    {
        public string Id { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public IFormFile? FormFile { get; set; }

        public string? Address { get; set; }

        public string? CountryId { get; set; }

        public IEnumerable<SelectListItem>? CountriesSelectList { get; set; }

        public string? TownId { get; set; }

        public IEnumerable<SelectListItem>? TownsSelectList { get; set; }
    }
}
