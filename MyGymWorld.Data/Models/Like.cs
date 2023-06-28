namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Like : BaseDeletableEntityModel
    {
        public Like()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;
    }
}
