using Microsoft.AspNetCore.Identity;
using MyGymWorld.Data.Common.Contracts;

namespace MyGymWorld.Data.Models
{
    public class ApplicationRole : IdentityRole<Guid>, ITimestampableModel, IDeletableModel
    {
        public ApplicationRole()
            : this(null)
        {
        }

        public ApplicationRole(string name)
            : base(name)
        {
            this.Id = Guid.NewGuid();
        }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
