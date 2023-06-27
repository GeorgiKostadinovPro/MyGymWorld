namespace MyGymWorld.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EventType
    {
        public EventType()
        {
            this.Id = Guid.NewGuid();

            this.Events = new HashSet<Event>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<Event> Events { get; set; }
    }
}
