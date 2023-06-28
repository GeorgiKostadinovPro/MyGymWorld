namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class ManagerGym : BaseDeletableEntityModel
    {
        public ManagerGym()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public Guid ManagerId { get; set; }

        public virtual Manager Manager { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;
    }
}
