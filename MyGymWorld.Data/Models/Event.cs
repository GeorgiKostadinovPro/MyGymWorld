namespace MyGymWorld.Data.Models
{
    using System;

    public class Event
    {
        public Event()
        {
            this.Id = Guid.NewGuid();
        } 
        
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Description { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public Guid EventTypeId { get; set; }

        public virtual EventType EventType { get; set; } = null!;
    }
}
