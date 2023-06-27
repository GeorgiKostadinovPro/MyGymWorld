namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class GymAddress : BaseDeletableEntityModel
    {
        public GymAddress()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public Guid AddressId { get; set; }

        public virtual Address Address { get; set; } = null!;
    }
}
