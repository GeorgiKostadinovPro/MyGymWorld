namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using MyGymWorld.Data.Models.Enums;
    using System;

    public class Manager : BaseDeletableEntityModel
    {
        public Manager()
        {
            this.Id = Guid.NewGuid();

            this.ManagersGyms = new HashSet<ManagerGym>();
        }
        public Guid Id { get; set; }

        public ManagerType ManagerType { get; set; }

        public bool IsCreator { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<ManagerGym> ManagersGyms { get; set; }
    }
}
