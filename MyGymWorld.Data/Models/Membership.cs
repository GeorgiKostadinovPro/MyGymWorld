namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using MyGymWorld.Data.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class Membership : BaseDeletableEntityModel
    {
        public Membership()
        {
            this.Id = Guid.NewGuid();

            this.UsersMemberships = new HashSet<UserMembership>();
        }
        public Guid Id { get; set; }

        public decimal Price { get; set; }

        public MembershipType MembershipType { get; set; }

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public virtual ICollection<UserMembership> UsersMemberships { get; set; }
    }
}
