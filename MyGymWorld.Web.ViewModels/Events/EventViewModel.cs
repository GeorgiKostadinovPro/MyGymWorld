namespace MyGymWorld.Web.ViewModels.Events
{
    public class EventViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string LogoUri { get; set; } = null!;

        public string CreatedOn { get; set; } = null!;
        
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
