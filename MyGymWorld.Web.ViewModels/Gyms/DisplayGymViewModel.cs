namespace MyGymWorld.Web.ViewModels.Gyms
{
    public class DisplayGymViewModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!; 
        
        public string LogoUri { get; set; } = null!;

        public string CreatedOn { get; set; } = null!;

        public int TotalDays { get; set; }
    }
}
