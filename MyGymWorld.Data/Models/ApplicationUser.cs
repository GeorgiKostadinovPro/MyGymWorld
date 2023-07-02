namespace MyGymWorld.Data.Models
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Common.Contracts;

    public class ApplicationUser : IdentityUser<Guid>, ITimestampableModel, IDeletableModel
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid();

            this.UsersGyms = new HashSet<UserGym>();
            this.UsersEvents = new HashSet<UserEvent>();
            this.UsersArticles = new HashSet<UserArticle>();
            this.UsersMemberships = new HashSet<UserMembership>();
            this.UsersChatGroups = new HashSet<UserChatGroup>();

            this.Likes = new HashSet<Like>();
            this.Dislikes = new HashSet<Dislike>();
            this.Comments = new HashSet<Comment>();
            this.Messages = new HashSet<Message>(); 
        }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        
        public Guid? AddressId { get; set; }
        
        public virtual Address Address { get; set; } = null!;

        public Guid? ManagerId { get; set; }

        public virtual Manager Manager { get; set; } = null!;

        public virtual ICollection<UserGym> UsersGyms { get; set; }

        public virtual ICollection<UserEvent> UsersEvents { get; set; }

        public virtual ICollection<UserArticle> UsersArticles { get; set; }

        public virtual ICollection<UserMembership> UsersMemberships { get; set; }

        public virtual ICollection<UserChatGroup> UsersChatGroups { get; set; }

        public virtual ICollection<Like> Likes { get; set; }

        public virtual ICollection<Dislike> Dislikes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
