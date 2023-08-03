namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;

    [TestFixture]
    public class AddressServiceTests
    {
        private MyGymWorldDbContext dbContext;

        [SetUp]
        public void Setup()
        {
            DbContextOptions<MyGymWorldDbContext> _options = new DbContextOptionsBuilder<MyGymWorldDbContext>()
                      .UseInMemoryDatabase(databaseName: "TestDb")
                      .Options;

            this.dbContext = new MyGymWorldDbContext(_options);
            this.dbContext.Database.EnsureCreated();
            this.dbContext.Database.EnsureDeleted();
        }

        [Test]
        public async Task CreateAddressAsyncShouldWorkProperly()
        {
            var mockRepository = new Mock<IRepository>();

            var service = new AddressService(mockRepository.Object);

            var addressName = "Sample Address";
            var townId = Guid.NewGuid().ToString();
            var address = await service.CreateAddressAsync(addressName, townId);

            Assert.IsNotNull(address);
            Assert.That(address.Name, Is.EqualTo(addressName));

            mockRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAddressByNameShouldWorkProperly()
        {
            var mockRepository = new Mock<IRepository>();

            await dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Cherni vrah",
                CreatedOn = DateTime.UtcNow,
            });

            await dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Bulgaria 1",
                CreatedOn = DateTime.UtcNow,
            });

            await dbContext.SaveChangesAsync();

            mockRepository
                .Setup(x => x.AllNotDeleted<Address>())
                .Returns(dbContext.Addresses.AsQueryable());

            var service = new AddressService(mockRepository.Object);

            var result = await service.GetAddressByNameAsync("bul. Cherni vrah");

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("bul. Cherni vrah"));
        }

        [Test]
        public async Task GetAddressByIdShouldWorkProperly()
        {
            var mockRepository = new Mock<IRepository>();

            Address address1 = new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Cherni vrah",
                CreatedOn = DateTime.UtcNow
            };

            Address address2 = new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Bulgaria 1",
                CreatedOn = DateTime.UtcNow
            };

            await dbContext.Addresses.AddRangeAsync(new List<Address> { address1, address2 });
            await dbContext.SaveChangesAsync();

            mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Address>())
                .Returns(dbContext.Addresses.AsQueryable());

            var service = new AddressService(mockRepository.Object);

            string testId = address1.Id.ToString();

            var result = await service.GetAddressByIdAsync(testId);

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("bul. Cherni vrah"));
        }
    }
}