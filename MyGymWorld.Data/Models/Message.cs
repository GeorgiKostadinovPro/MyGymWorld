namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class Message : BaseDeletableEntityModel
    {
        public Message()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Content { get; set; } = null!;

        public bool IsSeen { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid ChatGroupId { get; set; }

        public virtual ChatGroup ChatGroup { get; set; } = null!;
    }
}
