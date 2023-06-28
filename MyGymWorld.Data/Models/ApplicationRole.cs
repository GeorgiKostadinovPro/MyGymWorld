using Microsoft.AspNetCore.Identity;

namespace MyGymWorld.Data.Models
{
    public class ApplicationRole : IdentityRole<Guid>
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
    }
}
