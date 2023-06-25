using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyGymWorld.Data.Models;

namespace MyGymWorld.Data
{
    public class MyGymWorldDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public MyGymWorldDbContext(DbContextOptions<MyGymWorldDbContext> options)
            : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
