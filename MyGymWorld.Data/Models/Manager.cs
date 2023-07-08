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

            this.Gyms = new HashSet<Gym>();
            this.ChatGroups = new HashSet<ChatGroup>();
        }
        public Guid Id { get; set; }

        public ManagerType ManagerType { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public bool IsCreator { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<Gym> Gyms { get; set; }

        public virtual ICollection<ChatGroup> ChatGroups { get; set; }
    }
}
