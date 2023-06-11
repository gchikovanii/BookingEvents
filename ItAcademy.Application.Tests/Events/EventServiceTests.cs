using Moq;
using ItAcademy.Application.Events.Repositories;
using ItAcademy.Application.Events;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Application.Events.Request;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using Mapster;

namespace ItAcademy.Application.Tests.Events
{
    public class EventServiceTests
    {
        private readonly EventRequest _eventRequest;
        public EventServiceTests()
        {
            _eventRequest = GetEventRequest();
        }
        [Fact]
        public async void AddEvent_WhenDataIsCorrect_ShouldReturnStringId()
        {
            var userId = "UserId";
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.AddEvent(token, It.IsAny<Event>(), userId)).ReturnsAsync(userId);
            var newId = await service.AddEvent(token, _eventRequest, userId).ConfigureAwait(false);
            Assert.Equal(userId, newId);
        }
        [Fact]
        public async void AddEvent_WhenInputTimeIsBad_ShouldThrowException()
        {

            var userId = "UserId";
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.AddEvent(token, It.IsAny<Event>(), userId)).ReturnsAsync(userId);

            var task = async () => await service.AddEvent(token, GetBadTimeEventRequest(), userId).ConfigureAwait(false);

            await Assert.ThrowsAnyAsync<IncorrectInfoException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void DeleteEvent_WhenInpuIsRight_ShouldReturnTrue()
        {
            var eventId = 3;
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.DeleteEvent(token,eventId)).ReturnsAsync(true);
            var result =  await service.RemoveEvent(token, eventId).ConfigureAwait(false);
            Assert.True(result);
        }
        [Fact]
        public async void DeleteEvent_WhenInpuIsNotRight_ShouldThrowException()
        {
            var eventId = 3;
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.DeleteEvent(token, eventId)).ReturnsAsync(true);
            var task = async () => await service.RemoveEvent(token, 4).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void GetEvent_ShouldReturnResponse()
        {
            var eventId = 3;

            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetEventById(token, eventId)).ReturnsAsync(new Event
            {
                Id = 3,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 5,
                Quantity = 10,
                Title = "Event titleee",
                Status = true,
                Approved = true,
                UserId = "UserId"
            });
            var result = await service.GetEvent(token, eventId).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }
        [Fact]
        public async void GetEvent_WhenEventIdNotRight_ShouldThrowException()
        {
            var eventId = 3;
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetEventById(token, 5)).ReturnsAsync(new Event
            {
                Id = 5,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 5,
                Quantity = 10,
                Title = "Event titleee",
                Status = true,
                Approved = true,
                UserId = "UserId"
            });
            
            var task = async () => await service.GetEvent(token, eventId).ConfigureAwait(false); 
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void UpdateEvent_ShouldReturnResponse()
        {
            var eventId = 3;
            var userId = "UserId";

            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateEvent(token, It.IsAny<Event>(), eventId,userId)).ReturnsAsync(new Event
            {
                Id = 3,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 15,
                Quantity = 225,
                Title = "Event titleee",
                Status = true,
                Approved = true,
            });
            var result = await service.UpdateEvent(token, _eventRequest.Adapt<UpdateEventRequest>(), eventId, userId).ConfigureAwait(false);
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }

        [Fact]
        public async void UpdateEvent_WhenIdIsDifferent_ShouldReturnException()
        {
            var eventId = 3;
            var userId = "UserId";

            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateEvent(token, It.IsAny<Event>(), eventId, userId)).ReturnsAsync(new Event
            {
                Id = 3,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 15,
                Quantity = 225,
                Title = "Event titleee",
                Status = true,
                Approved = true,
            });
            var task = async () => await service.UpdateEvent(token, _eventRequest.Adapt<UpdateEventRequest>(), eventId, "anabana").ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesNotHaveAccessException>(task).ConfigureAwait(false);
        }
        [Fact]
        public async void UpdateEvent_WheneEventIdIsZero_ShouldReturnException()
        {
            var eventId = 0;
            var userId = "UserId";

            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateEvent(token, It.IsAny<Event>(), eventId, userId)).ReturnsAsync(new Event
            {
                Id = 0,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 15,
                Quantity = 225,
                Title = "Event titleee",
                Status = true,
                Approved = true,
            });
            var task = async () => await service.UpdateEvent(token, EventRequestWithIdZero().Adapt<UpdateEventRequest>(), eventId, userId).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void UpdateEvent_WheneUserIdIsNull_ShouldReturnException()
        {
            var eventId = 3;
            var mockRepository = new Mock<IEventRepository>();
            var service = new EventService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateEvent(token, It.IsAny<Event>(), eventId, null)).ReturnsAsync(new Event
            {
                Id = 3,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(3),
                Image = "image url.....",
                Price = 15,
                Quantity = 225,
                Title = "Event titleee",
                Status = true,
                Approved = true,
            });
            var task = async () => await service.UpdateEvent(token, EventRequestWithIdZero().Adapt<UpdateEventRequest>(), eventId, null).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync <NullReferenceException>(task).ConfigureAwait(false);
        }

        private static EventRequest GetEventRequest()
        {
            return new EventRequest
            {
                Id = 1,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Image = "image url.....",
                Price = 5,
                Quantity = 10,
                Title = "Event titleee",
            };
        }
        private static EventRequest GetBadTimeEventRequest()
        {
            return new EventRequest
            {
                Id = 1,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now,
                Image = "image url.....",
                Price = 5,
                Quantity = 10,
                Title = "Event titleee",
            };
        }
        private static EventRequest EventRequestWithIdZero()
        {
            return new EventRequest
            {
                Id = 0,
                Description = "Random Text",
                Location = "Tbilisi Georgia",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(5),
                Image = "image url.....",
                Price = 5,
                Quantity = 10,
                Title = "Event titleee",
            };
        }
    }
}
