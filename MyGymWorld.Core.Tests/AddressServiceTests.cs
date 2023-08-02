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
            var dbContext = CreateContext();

            var service = new AddressService(mockRepository.Object);

            var addressName = "Sample Address";
            var townId = Guid.NewGuid().ToString();
            var address = await service.CreateAddressAsync(addressName, townId);

            Assert.IsNotNull(address);
            Assert.AreEqual(addressName, address.Name);

            mockRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
            mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}