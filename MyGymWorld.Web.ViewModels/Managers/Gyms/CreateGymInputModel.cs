namespace MyGymWorld.Web.ViewModels.Managers.Gyms
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Common;
    using System.ComponentModel.DataAnnotations;

    public class CreateGymInputModel
    {
        [Required]
        [StringLength(ValidationalConstants.GymConstants.NameMaxLength, ErrorMessage = "The name must be at least 5 and at max 100 characters long.",
            MinimumLength = ValidationalConstants.GymConstants.NameMinLength)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        [RegularExpression("\\+[0-9]{1,3}-[0-9]{1,10}", ErrorMessage = "Your phone number must follow the pattern above")]
        [StringLength(ValidationalConstants.GymConstants.PhoneNumberMaxLength, ErrorMessage = "The phone number must be at least 7 and at max 15 characters long.",
         MinimumLength = ValidationalConstants.GymConstants.PhoneNumberMinLength)]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [Display(Name = "Logo")]
        public IFormFile LogoFile { get; set; } = null!;

        [Display(Name = "Gallery")]
        public IEnumerable<IFormFile>? GalleryImagesFiles { get; set; }

        [Required]
        [Display(Name = "Webiste Url")]
        public string WebsiteUrl { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string GymType { get; set; } = null!;

        [Display(Name = "Gym Types")]
        public IEnumerable<string>? GymTypes { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public string CountryId { get; set; } = null!;

        [Display(Name = "Countries")]
        public IEnumerable<SelectListItem>? CountriesSelectList { get; set; }

        [Required]
        public string TownId { get; set; } = null!;

        [Display(Name = "Towns")]
        public IEnumerable<SelectListItem>? TownsSelectList { get; set; }

        public ImageUploadResult LogoParams { get; set; } = null!;

        public ICollection<ImageUploadResult> GalleryImagesParams { get; set; } = new HashSet<ImageUploadResult>();
    }
}
