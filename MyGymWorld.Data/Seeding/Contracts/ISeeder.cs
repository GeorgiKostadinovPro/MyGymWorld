using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGymWorld.Data.Seeding.Contracts
{
    public interface ISeeder
    {
        Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider);
    }
}
