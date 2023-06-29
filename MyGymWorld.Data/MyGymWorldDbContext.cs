using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyGymWorld.Data.Models;
using System.Reflection;

namespace MyGymWorld.Data
{
    public class MyGymWorldDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public MyGymWorldDbContext(DbContextOptions<MyGymWorldDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<Address> Addresses { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Article> Articles { get; set; }

        public DbSet<ArticleCategory> ArticlesCategories { get; set; }

        public DbSet<UserArticle> UsersArticles { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<UserEvent> UsersEvents { get; set; }

        public DbSet<GymImage> GymImages { get; set; }
        
        public DbSet<Gym> Gyms { get; set; }

        public DbSet<GymAddress> GymAddresses { get; set; }

        public DbSet<UserGym> UsersGyms { get; set; }

        public DbSet<Manager> Managers { get; set; }

        public DbSet<ManagerGym> ManagersGyms { get; set; }

        public DbSet<Like> Likes { get; set; }

        public DbSet<Dislike> Dislikes { get; set; }

        public DbSet<Comment> Comments { get; set; }
        
        public DbSet<Membership> Memberships { get; set; }

        public DbSet<UserMembership> UsersMemberships { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(MyGymWorldDbContext))
                ?? Assembly.GetExecutingAssembly();

            builder.ApplyConfigurationsFromAssembly(assembly);

            base.OnModelCreating(builder);
        }
    }
}
