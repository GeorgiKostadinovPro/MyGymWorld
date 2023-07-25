﻿namespace MyGymWorld.Web.ViewModels.Events
{
    public class EventDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string OrganiserId { get; set; } = null!;

        public string Organiser { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string GymName { get; set; } = null!;

        public string LogoUri { get; set; } = null!;

        public string CreatedOn { get; set; } = null!;
    }
}
