namespace MyGymWorld.Core.Tests
{
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;

    [TestFixture]
    public class AddressServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private Mock<IRepository> mockRepository;

        [SetUp]
        public async Task Setup()
        {
            this.mockRepository = new Mock<IRepository>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateAddressAsyncShouldWorkProperly()
        {
            var service = new AddressService(this.mockRepository.Object);

            var addressName = "Sample Address";
            var townId = Guid.NewGuid().ToString();

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Address>()))
                .Callback(async (Address address) =>
                {
                    await this.dbContext.Addresses.AddAsync(address);
                    await this.dbContext.SaveChangesAsync();
                });

            var address = await service.CreateAddressAsync(addressName, townId);

            var count = await this.dbContext.Addresses.CountAsync();

            Assert.IsNotNull(address);
            Assert.That(address.Name, Is.EqualTo(addressName));
            Assert.That(count, Is.EqualTo(1));

            this.mockRepository.Verify(r => r.AddAsync(It.IsAny<Address>()), Times.Once);
            this.mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task GetAddressByIdShouldReturnNullWhenIdIsInvalid(string addressId)
        {
            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Cherni vrah",
                CreatedOn = DateTime.UtcNow
            });

            await this.dbContext.SaveChangesAsync();
            
            var mockRepository = new Mock<IRepository>();

            mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Address>())
                .Returns(dbContext.Addresses.AsQueryable());

            var service = new AddressService(mockRepository.Object);

            var result = await service.GetAddressByIdAsync(addressId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAddressByNameShouldWorkProperly()
        {
            await this.dbContext.Addresses.AddRangeAsync(new HashSet<Address>
            {
                new Address
                {
                    Id = Guid.NewGuid(),
                    Name = "bul. Cherni vrah",
                    CreatedOn = DateTime.UtcNow,
                },
                new Address
                {
                    Id = Guid.NewGuid(),
                    Name = "bul. Bulgaria 1",
                    CreatedOn = DateTime.UtcNow,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Address>())
                .Returns(dbContext.Addresses.AsQueryable());

            var service = new AddressService(this.mockRepository.Object);

            var result = await service.GetAddressByNameAsync("bul. Cherni vrah");

            Assert.IsNotNull(result);
            Assert.That(result.Name, Is.EqualTo("bul. Cherni vrah"));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public async Task GetAddressByNameShouldReturnNullWhenNameIsInvalid(string name)
        {
            await this.dbContext.Addresses.AddAsync(new Address
            {
                Id = Guid.NewGuid(),
                Name = "bul. Cherni vrah",
                CreatedOn = DateTime.UtcNow,
            });

            await this.dbContext.SaveChangesAsync();
            
            this.mockRepository
                .Setup(x => x.AllNotDeleted<Address>())
                .Returns(this.dbContext.Addresses.AsQueryable());

            var service = new AddressService(this.mockRepository.Object);

            var result = await service.GetAddressByNameAsync(name);

            Assert.IsNull(result);
        }
    }
}