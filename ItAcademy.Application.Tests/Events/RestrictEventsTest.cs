using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using Moq;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Application.Manage;
using ItAcademy.Domain.ManageAggregate;

namespace ItAcademy.Application.Tests.Events
{
    public class RestrictEventsTest
    {
        [Fact]
        public async void GetRestrictEvent_ShouldReturnHours()
        {
            var id = 1;
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetRestrictionTime(token)).ReturnsAsync(id);
            var result = await service.GetRestrictionTime(token).ConfigureAwait(false);
            Assert.Equal(id, result);
        }
        [Fact]
        public async void GetRestrictEvent_WhenIdIsNotCorrect_ShouldThrowException()
        {
            var id = 5;
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetRestrictionTime(token)).ReturnsAsync(id);
            var task = async () => await service.GetRestrictionTime(token).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void GetReserveTime_ShouldReturnMinutes()
        {
            var id = 1;
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetReservationTime(token)).ReturnsAsync(id);
            var result = await service.GetReservationTime(token).ConfigureAwait(false);
            Assert.Equal(id, result);
        }
        [Fact]
        public async void GetReserveEvent_WhenIdIsNotCorrect_ShouldThrowException()
        {
            var id = 9;
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);

            var token = new CancellationToken();
            mockRepository.Setup(i => i.GetReservationTime(token)).ReturnsAsync(id);
            var task = async () => await service.GetReservationTime(token).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }
        [Fact]
        public async void UpdateEventRestrictionTime_WhenIdNotRight_ShouldThrowException()
        {
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateRestrictionTime(token, GetRestrictionTime())).Returns(Task.FromResult(true));
            var task = async () => await service.UpdateRestrictionTimeInHours(token, GetRestrictionTime()).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        [Fact]
        public async void UpdateEventReservationTime_ShouldReturnResponse()
        {
            var mockRepository = new Mock<IManageTimeRepository>();
            var service = new ManageTimeService(mockRepository.Object);
            var token = new CancellationToken();
            mockRepository.Setup(i => i.UpdateReservationTime(token, BadReservationTime())).Returns(Task.FromResult(true));

            var task = async () => await service.UpdateReservationTimeInMinutes(token, BadReservationTime()).ConfigureAwait(false);
            await Assert.ThrowsAnyAsync<DoesntExistsException>(task).ConfigureAwait(false);
        }

        private static RestrictEventTime GetRestrictionTime()
        {
            return new RestrictEventTime
            {
                Id = 5,
                Hours = 7
            };
        }
        private static ReserveTime BadReservationTime()
        {
            return new ReserveTime
            {
                Id = 5,
                Minutes = 7
            };
        }
    }
}
