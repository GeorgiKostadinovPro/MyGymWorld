namespace MyGymWorld.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Common.Contracts;
    using MyGymWorld.Data.Common.Models;

    public class ApplicationUser : IdentityUser<Guid>, ITimestampableModel, IDeletableModel
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();

            this.UsersGyms = new HashSet<UserGym>();
        }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        
        public Guid AddressId { get; set; }
        
        public virtual Address Address { get; set; } = null!;

        public virtual ICollection<UserGym> UsersGyms { get; set; }
    }
}
