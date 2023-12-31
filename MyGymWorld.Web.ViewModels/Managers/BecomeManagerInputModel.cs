﻿namespace MyGymWorld.Web.ViewModels.Managers
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class BecomeManagerInputModel
    {
        public BecomeManagerInputModel()
        {
            this.ManagerTypes = new HashSet<string>();
        }

        public string Id { get; set; } = null!;

        [Required]
        [StringLength(ApplicationUserConstants.UsernameMaxLength, ErrorMessage = "The username must be at least 5 and at max 50 characters long.",
            MinimumLength = ApplicationUserConstants.UsernameMinLength)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(ApplicationUserConstants.FirstNameMaxLength, ErrorMessage = "The firstname must be at least 3 and at max 20 characters long.",
           MinimumLength = ApplicationUserConstants.FirstNameMinLength)]
        [Display(Name = "First name")]

        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(ApplicationUserConstants.LastNameMaxLength, ErrorMessage = "The lastname must be at least 3 and at max 20 characters long.",
           MinimumLength = ApplicationUserConstants.LastNameMinLength)]
        [Display(Name = "Last name")]

        public string LastName { get; set; } = null!;

        [Required]
        [Phone]
        [RegularExpression("\\+[0-9]{1,3}-[0-9]{1,10}", ErrorMessage = "Your phone number must follow the pattern above")]
        [StringLength(ApplicationUserConstants.PhoneNumberMaxLength, ErrorMessage = "The phone number must be at least 5 and at max 15 characters long.",
            MinimumLength = ApplicationUserConstants.PhoneNumberMinLength)]
        public string PhoneNumber { get; set; } = null!;

        public bool IsApproved { get; set; }

        public string ManagerType { get; set; } = null!;

        public IEnumerable<string> ManagerTypes { get; set; }    
    }
}
