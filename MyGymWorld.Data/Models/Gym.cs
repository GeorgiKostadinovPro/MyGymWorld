namespace MyGymWorld.Data.Models
{
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using MyGymWorld.Data.Common.Models;
    using MyGymWorld.Data.Models.Enums;

    public class Gym : BaseDeletableEntityModel
    {
        public Gym()
        {
            this.Id = Guid.NewGuid();
           
            this.GymImages = new HashSet<GymImage>();
            this.GymsAddresses = new HashSet<GymAddress>(); 
            this.UsersGyms = new HashSet<UserGym>();
            this.ManagersGyms = new HashSet<ManagerGym>();

            this.Events = new HashSet<Event>();
            this.Articles = new HashSet<Article>();
            this.Likes = new HashSet<Like>();
            this.Dislikes = new HashSet<Dislike>();
            this.Comments = new HashSet<Comment>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string LogoUrl { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!; 

        public GymType GymType { get; set; }
        
        public virtual ICollection<GymImage> GymImages { get; set; }

        public virtual ICollection<GymAddress> GymsAddresses { get; set; }
       
        public virtual ICollection<UserGym> UsersGyms { get; set; } 

        public virtual ICollection<ManagerGym> ManagersGyms { get; set; }

        public virtual ICollection<Event> Events { get; set; } 

        public virtual ICollection<Article> Articles { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<Dislike> Dislikes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
    }
}
