namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class UserMembership : BaseDeletableEntityModel
    {
        public UserMembership()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public DateTime ValidTo { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid MembershipId { get; set; }

        public virtual Membership Membership { get; set; } = null!;
    }
}
