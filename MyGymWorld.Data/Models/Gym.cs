namespace MyGymWorld.Data.Models
{
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using MyGymWorld.Data.Common.Models;

    public class Gym : BaseDeletableEntityModel
    {
        public Gym()
        {
            this.Id = Guid.NewGuid();
           
            this.GymImages = new HashSet<GymImage>();
            this.GymsAddresses = new HashSet<GymAddress>(); 
            this.UsersGyms = new HashSet<UserGym>();            
            this.Events = new HashSet<Event>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string LogoUrl { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!; 
        
        public virtual ICollection<GymImage> GymImages { get; set; }

        public virtual ICollection<GymAddress> GymsAddresses { get; set; }
       
        public virtual ICollection<UserGym> UsersGyms { get; set; } 

        public virtual ICollection<Event> Events { get; set; } 
    }
}
