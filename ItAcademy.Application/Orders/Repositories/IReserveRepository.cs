using ItAcademy.Application.Orders.Requests;
using ItAcademy.Domain.OrderAggregate;

namespace ItAcademy.Application.Orders.Repositories
{
    public interface IReserveRepository
    {
        Task<bool> MakeReserve(CancellationToken token, ReservationRequest reservation, int eventId);
        Task<bool> MakeOrder(CancellationToken token, int reservationId);
        Task TerminateOrder(CancellationToken token);
        Task<Reservation> GetReservation(CancellationToken token, string userId);
    }
}
