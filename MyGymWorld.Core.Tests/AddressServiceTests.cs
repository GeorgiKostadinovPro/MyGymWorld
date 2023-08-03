namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System.Security.Cryptography.X509Certificates;

    [TestFixture]
    public class AddressServiceTests
    {
        private readonly DbContextOptions<MyGymWorldDbContext> _options = new DbContextOptionsBuilder<MyGymWorldDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
          .Options;

        private MyGymWorldDbContext CreateContext()
        {
            return new MyGymWorldDbContext(_options);
        }

        [SetUp]
        public void Setup()
        {
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
            
            var dbContext = CreateContext();

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

            mockRepository.Setup(x => x.All<Address>())
                .Returns(dbContext.Addresses.AsQueryable());

            var service = new AddressService(mockRepository.Object);

            var result = await service.GetAddressByNameAsync("bul. Cherni vrah");

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("bul. Cherni vrah"));
        }
    }
}