using ItAcademy.Domain.ManageAggregate;

namespace ItAcademy.Application.Manage.Repositories
{
    public interface IManageTimeRepository
    {
        Task<int> GetReservationTime(CancellationToken token);
        Task<int> GetRestrictionTime(CancellationToken token);
        Task<bool> UpdateReservationTime(CancellationToken token, ReserveTime time);
        Task<bool> UpdateRestrictionTime(CancellationToken token, RestrictEventTime time);
    }
}
