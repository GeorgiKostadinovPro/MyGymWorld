namespace MyGymWorld.Web.ViewModels.Events
{
    public class EventViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string OrganiserId { get; set; } = null!;

        public string Organiser { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string GymName { get; set;} = null!;

        public DateTime CreatedOn { get; set; }
    }
}
