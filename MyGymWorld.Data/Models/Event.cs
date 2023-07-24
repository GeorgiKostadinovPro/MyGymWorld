namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using MyGymWorld.Data.Models.Enums;
    using System;

    public class Event : BaseDeletableEntityModel
    {
        public Event()
        {
            this.Id = Guid.NewGuid();

            this.UsersEvents = new HashSet<UserEvent>();
        } 
        
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public EventType EventType { get; set; }

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public virtual ICollection<UserEvent> UsersEvents { get; set; }
    }
}
