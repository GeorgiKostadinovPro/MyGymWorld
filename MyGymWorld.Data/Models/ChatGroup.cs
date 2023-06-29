namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class ChatGroup : BaseDeletableEntityModel
    {
        public ChatGroup()
        {
            this.Id = Guid.NewGuid();

            this.UsersChatGroups = new HashSet<UserChatGroup>();
            this.Messages = new HashSet<Message>();
        }

        public Guid Id { get; set; }

        public Guid ManagerId { get; set; }

        public virtual Manager Manager { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public virtual ICollection<UserChatGroup> UsersChatGroups { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
