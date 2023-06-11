using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Domain.ManageAggregate;
using ItAcademy.Infrastructure.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Infrastructure.Manage
{
    public class ManageTimeRepository : IManageTimeRepository
    {
        private readonly IBaseRepository<ReserveTime> _reservationTimeRepository;
        private readonly IBaseRepository<RestrictEventTime> _restrictTimeRepository;
        public ManageTimeRepository(IBaseRepository<ReserveTime> reservationTimeRepository, IBaseRepository<RestrictEventTime> restrictTimeRepository)
        {
            _reservationTimeRepository = reservationTimeRepository;
            _restrictTimeRepository = restrictTimeRepository;
        }

        public async Task<int> GetReservationTime(CancellationToken token)
        {
            var result = await _reservationTimeRepository.GetQuery(i => i.Id == 1).FirstOrDefaultAsync(token).ConfigureAwait(false);
            return result.Minutes;
        }
        public async Task<int> GetRestrictionTime(CancellationToken token)
        {
            var result = await _restrictTimeRepository.GetQuery(i => i.Id == 1).FirstOrDefaultAsync(token).ConfigureAwait(false);
            return result.Hours;
        }

        public async Task<bool> UpdateReservationTime(CancellationToken token, ReserveTime time)
        {
            time.Id = 1;
            _reservationTimeRepository.Update(time);
            return await _reservationTimeRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }
        public async Task<bool> UpdateRestrictionTime(CancellationToken token, RestrictEventTime time)
        {
            time.Id = 1;
            _restrictTimeRepository.Update(time);
            return await _restrictTimeRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }
    }
}
