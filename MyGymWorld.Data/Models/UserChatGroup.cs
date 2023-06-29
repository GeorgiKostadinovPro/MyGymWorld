namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class UserChatGroup : BaseDeletableEntityModel
    {
        public UserChatGroup()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid ChatGroupId { get; set; }

        public virtual ChatGroup ChatGroup { get; set; } = null!;
    }
}
