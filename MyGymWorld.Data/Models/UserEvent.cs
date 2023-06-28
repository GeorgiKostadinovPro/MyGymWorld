namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class UserEvent : BaseDeletableEntityModel
    {
        public UserEvent()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid EventId { get; set; }

        public virtual Event Event { get; set; } = null!;
    }
}
