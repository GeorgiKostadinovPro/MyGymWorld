namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class UserGym : BaseDeletableEntityModel
    {
        public UserGym() 
        { 
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public bool IsSubscribedForEvents { get; set; }

        public bool IsSubscribedForArticles { get; set; }

        public bool IsNotifiedByEmail { get; set; }

        public bool IsNotifiedBySMS { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;
    }
}
