namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Address : BaseDeletableEntityModel
    {
        public Address()
        {
            this.Id = Guid.NewGuid();

            this.Users = new HashSet<ApplicationUser>();
            this.GymsAddresses = new HashSet<GymAddress>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public Guid TownId { get; set; }

        public virtual Town Town { get; set; } = null!;

        public virtual ICollection<ApplicationUser> Users { get; set; }

        public virtual ICollection<GymAddress> GymsAddresses { get; set; }
    }
}