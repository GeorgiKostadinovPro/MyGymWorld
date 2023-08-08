namespace MyGymWorld.Core.Tests
{
    using AutoMapper;
    using Moq;
    using MyGymWorld.Core.Tests.Helpers;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Data;
    using System.Threading.Tasks;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Events;
    using MyGymWorld.Core.Services;
    using MyGymWorld.Data.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using MyGymWorld.Common;

    [TestFixture]
    public class EventServiceTests
    {
        private MyGymWorldDbContext dbContext;

        private IMapper mapper;
        private Mock<IRepository> mockRepository;

        [SetUp]
        public async Task Setup()
        {
            this.mapper = InitializeAutoMapper.CreateMapper();

            this.mockRepository = new Mock<IRepository>();

            this.dbContext = await InitializeInMemoryDatabase.CreateInMemoryDatabase();
        }

        [Test]
        public async Task CreateEventAsyncShouldWorkProperly()
        {
            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<Event>()))
                .Callback(async (Event @event) =>
                {
                    await this.dbContext.Events.AddAsync(@event);
                    await this.dbContext.SaveChangesAsync();
                });

            CreateEventInputModel createEventInputModel = new CreateEventInputModel
            {
                GymId = Guid.NewGuid().ToString(),
                Name = "Test",
                Description = "Test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(1),
                EventType = "Business"
            };

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.CreateEventAsync(createEventInputModel);

            Assert.IsNotNull(result);
            Assert.That(result.Description, Is.EqualTo("Test"));
        }

        [Test]
        public async Task EditEventAsyncShouldWorkProperly()
        {
            var eventId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.Parse(eventId),
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Event>())
                .Returns(this.dbContext.Events
                .Where(e => e.IsDeleted == false));

            EditEventInputModel editEventInputModel = new EditEventInputModel
            {
                Id = eventId,
                GymId = Guid.NewGuid().ToString(),
                Name = "Test Edited",
                Description = "Test",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(1),
                EventType = "Business"
            };

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.EditEventAsync(eventId, editEventInputModel);

            var editedEvent = await this.dbContext.Events
                .FirstAsync(e => e.Id.ToString() == eventId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(editedEvent.Name, Is.EqualTo("Test Edited"));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task EditEventAsyncShouldNotDoAnythingWhenIdIsInvalid(string eventId)
        {
            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Event>())
                .Returns(this.dbContext.Events
                .Where(e => e.IsDeleted == false));

            EditEventInputModel editEventInputModel = new EditEventInputModel();

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.EditEventAsync(eventId, editEventInputModel);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task DeleteEventAsyncShouldWorkProperly()
        {
            var eventId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.Parse(eventId),
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Event>())
                .Returns(this.dbContext.Events
                .Where(e => e.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.DeleteEventAsync(eventId);

            var deletedEvent = await this.dbContext.Events
                .FirstAsync(e => e.Id.ToString() == eventId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsTrue(deletedEvent.IsDeleted);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task DeleteEventAsyncShouldNotDoAnythingWhenIdIsInvalid(string eventId)
        {
            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeleted<Event>())
                .Returns(this.dbContext.Events
                .Where(e => e.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.DeleteEventAsync(eventId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Test]
        public async Task ParticipateInEventAsyncShouldCreateNewUserEventWhenUserParticipatesForTheFirstTime()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var eventId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            this.mockRepository
                 .Setup(x => x.All<UserEvent>())
                 .Returns(this.dbContext.UsersEvents.AsQueryable());

            this.mockRepository
                .Setup(x => x.AddAsync(It.IsAny<UserEvent>()))
                .Callback(async (UserEvent userEvent) =>
                {
                    await this.dbContext.UsersEvents.AddAsync(userEvent);
                    await this.dbContext.SaveChangesAsync();
                });

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.ParticipateInEventAsync(eventId, userId);

            var usersEventsCount = await this.dbContext.UsersEvents
                .CountAsync(ue => ue.IsDeleted == false);

            this.mockRepository.Verify(x => x.AddAsync(It.IsAny<UserEvent>()), Times.Once);
            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.That(usersEventsCount, Is.EqualTo(1));
        }

        [Test]
        public async Task ParticipateInEventAsyncShouldSetIsDeletedToFalseWhenUserParticipatedInEventBefore()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var eventId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.Parse(userId),
                EventId = Guid.Parse(eventId),
                IsDeleted = true
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.All<UserEvent>())
                 .Returns(this.dbContext.UsersEvents.AsQueryable());

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.ParticipateInEventAsync(eventId, userId);

            var userEvent = await this.dbContext.UsersEvents
                .FirstAsync(ue => ue.UserId.ToString() == userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsFalse(userEvent.IsDeleted);
        }

        [Test]
        public async Task LeaveEventAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var eventId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersEvents.AddAsync(new UserEvent
            {
                UserId = Guid.Parse(userId),
                EventId = Guid.Parse(eventId),
                IsDeleted = false
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.All<UserEvent>())
                 .Returns(this.dbContext.UsersEvents.AsQueryable());

            var service = new EventService(this.mapper, this.mockRepository.Object);

            await service.LeaveEventAsync(eventId, userId);

            var userEvent = await this.dbContext.UsersEvents
               .FirstAsync(ue => ue.UserId.ToString() == userId);

            this.mockRepository.Verify(x => x.SaveChangesAsync(), Times.Once);

            Assert.IsTrue(userEvent.IsDeleted);
        }

        [Test]
        public async Task LeaveEventAsyncShouldThrowExceptionWhenUserIsNotParticipatingInTheEvent()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";
            var eventId = "832fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            this.mockRepository
                 .Setup(x => x.All<UserEvent>())
                 .Returns(this.dbContext.UsersEvents.AsQueryable());

            var service = new EventService(this.mapper, this.mockRepository.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.LeaveEventAsync(eventId, userId);

            }, ExceptionConstants.EventErrors.EventNotJoinedToBeLeft);
        }

        [Test]
        public async Task GetAllActiveEventsCountShouldWorkProperly()
        {
            var gymId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    GymId = Guid.Parse(gymId),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = false
                },
                new Event
                {
                    GymId = Guid.Parse(gymId),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = false
                },
                new Event
                {
                    GymId = Guid.Parse(gymId),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<Event>())
                 .Returns(this.dbContext.Events
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveEventsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(2));
        }


        [Test]
        public async Task GetAllActiveEventsCountShouldReturnZeroWhenGymIdIsInvalid()
        {
            var gymId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = false
                },
                new Event
                {
                    GymId = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<Event>())
                 .Returns(this.dbContext.Events
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveEventsCountByGymIdAsync(gymId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllUserJoinedEventsCountAsyncShouldWorkProperly()
        {
            var userId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.UsersEvents.AddRangeAsync(new HashSet<UserEvent>
            {
                new UserEvent
                {
                    UserId = Guid.Parse(userId),
                    IsDeleted = false
                },
                new UserEvent
                {
                    UserId = Guid.Parse(userId),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();


            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<UserEvent>())
                 .Returns(this.dbContext.UsersEvents
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllUserJoinedEventsCountAsync(userId);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetAllUserJoinedEventsCountAsyncShouldReturnZeroWhenUserIdIsInvalid(string userId)
        {
            await this.dbContext.UsersEvents.AddRangeAsync(new HashSet<UserEvent>
            {
                new UserEvent
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = false
                },
                new UserEvent
                {
                    UserId = Guid.NewGuid(),
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();


            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<UserEvent>())
                 .Returns(this.dbContext.UsersEvents
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllUserJoinedEventsCountAsync(userId);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetEventForEditByIdAsyncShouldWorkProperly() 
        {
            var eventId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.Parse(eventId),
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 3",
                    Description = "Test 3",
                    IsDeleted = true
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<Event>())
                 .Returns(this.dbContext.Events
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetEventForEditByIdAsync(eventId);

            Assert.IsNotNull(result);
            Assert.That(result.Id, Is.EqualTo(eventId));
        }

        [Test]
        public async Task GetEventByIdAsyncShouldWorkProperly()
        {
            var eventId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.Parse(eventId),
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<Event>())
                 .Returns(this.dbContext.Events
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetEventByIdAsync(eventId);

            Assert.IsNotNull(result);
            Assert.That(result.Id.ToString(), Is.EqualTo(eventId));
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task GetEventByIdAsyncShouldReturnNullWhenIdIsInvalid(string eventId)
        {
            await this.dbContext.Events.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    GymId = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddHours(1),
                    EventType = EventType.Business,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                 .Setup(x => x.AllNotDeletedReadonly<Event>())
                 .Returns(this.dbContext.Events
                 .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetEventByIdAsync(eventId);

            Assert.IsNull(result);
        }

        [Test]
        public async Task CheckIfEventExistsByIdAsyncShouldWorkProperly()
        {
            var eventId = "932fe39a-bc5b-4ea4-b0c5-68b2da06768e";

            await this.dbContext.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.Parse(eventId),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 3",
                    Description = "Test 3",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Event>())
                .Returns(this.dbContext.Events
                .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.CheckIfEventExistsByIdAsync(eventId);

            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("932fe39a-bc5b-4ea4-b0c5-68b2da06768e")]
        public async Task CheckIfEventExistsByIdAsyncShouldTReturnFalseWhenIdIsInvalid(string eventId)
        {
            await this.dbContext.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Event>())
                .Returns(this.dbContext.Events
                .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.CheckIfEventExistsByIdAsync(eventId);

            Assert.IsFalse(result);
        }


        [Test]
        public async Task GetAllActiveEventsCountAsyncShouldWorkProperly()
        {
            await this.dbContext.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = false,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 3",
                    Description = "Test 3",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Event>())
                .Returns(this.dbContext.Events
                .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveEventsCountAsync();

            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllActiveEventsCountAsyncShouldReturnZeroWhenThereAreNoActiveEvents()
        {
            await this.dbContext.AddRangeAsync(new HashSet<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 1",
                    Description = "Test 1",
                    IsDeleted = true,
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Test 2",
                    Description = "Test 2",
                    IsDeleted = true,
                }
            });

            await this.dbContext.SaveChangesAsync();

            this.mockRepository
                .Setup(x => x.AllNotDeletedReadonly<Event>())
                .Returns(this.dbContext.Events
                .Where(x => x.IsDeleted == false));

            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = await service.GetAllActiveEventsCountAsync();

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void GetAllEventTypesShouldWorkProperly()
        {
            var service = new EventService(this.mapper, this.mockRepository.Object);

            var result = service.GetAllEventTypes();

            Assert.That(result.Count(), Is.EqualTo(4));
        }
    }
}
