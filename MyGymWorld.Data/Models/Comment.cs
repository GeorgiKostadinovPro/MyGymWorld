namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Comment : BaseDeletableEntityModel
    {
        public Comment()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Content { get; set; } = null!;
        
        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public Guid? ParentId { get; set; }

        public virtual Comment Parent { get; set; } = null!;
    }
}
