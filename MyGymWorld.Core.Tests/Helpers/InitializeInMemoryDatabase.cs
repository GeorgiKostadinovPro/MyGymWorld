namespace MyGymWorld.Core.Tests.Helpers
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Data;

    public static class InitializeInMemoryDatabase
    {
        public static async Task<MyGymWorldDbContext> CreateInMemoryDatabase()
        {
            DbContextOptions<MyGymWorldDbContext> _options = new DbContextOptionsBuilder<MyGymWorldDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb")
                      .Options;

            MyGymWorldDbContext dbContext = new MyGymWorldDbContext(_options);
            await dbContext.Database.EnsureCreatedAsync();
            await dbContext.Database.EnsureDeletedAsync();

            return dbContext;
        }
    }
}
