using ItAcademy.Domain.ManageAggregate;

namespace ItAcademy.Application.Manage
{
    public interface IManageTimeService
    {
        Task<int> GetReservationTime(CancellationToken token);
        Task<int> GetRestrictionTime(CancellationToken token);
        Task<bool> UpdateReservationTimeInMinutes(CancellationToken token, ReserveTime time);
        Task<bool> UpdateRestrictionTimeInHours(CancellationToken token, RestrictEventTime time);
    }
}
