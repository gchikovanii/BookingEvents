using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Domain.ManageAggregate;

namespace ItAcademy.Application.Manage
{
    public class ManageTimeService : IManageTimeService
    {
        private readonly IManageTimeRepository _repository;

        public ManageTimeService(IManageTimeRepository repository) => _repository = repository;

        public async Task<int> GetReservationTime(CancellationToken token)
        {
            var result = await _repository.GetReservationTime(token).ConfigureAwait(false);
            return result;
        }
        public async Task<int> GetRestrictionTime(CancellationToken token)
        {
            var result = await _repository.GetRestrictionTime(token).ConfigureAwait(false);
            return result;
        }
        public async Task<bool> UpdateReservationTimeInMinutes(CancellationToken token, ReserveTime time)
        {
            return await _repository.UpdateReservationTime(token, time).ConfigureAwait(false);
        }
        public async Task<bool> UpdateRestrictionTimeInHours(CancellationToken token, RestrictEventTime time)
        {
            return await _repository.UpdateRestrictionTime(token, time).ConfigureAwait(false);
        }
    }
}
