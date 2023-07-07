namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class Notification : BaseDeletableEntityModel
    {
        public Notification()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsRead { get; set; }
    }
}
