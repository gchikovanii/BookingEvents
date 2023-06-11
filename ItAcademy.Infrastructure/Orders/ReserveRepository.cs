using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Application.Orders.Repositories;
using ItAcademy.Application.Orders.Requests;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.OrderAggregate;
using ItAcademy.Domain.UserAggregate;
using ItAcademy.Infrastructure.BaseRepo;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Infrastructure.Orders
{
    public class ReserveRepository : IReserveRepository
    {
        #region Ctor
        private readonly IBaseRepository<Reservation> _reserveRepository;
        private readonly IBaseRepository<Order> _orderRepository;
        private readonly IBaseRepository<AppUser> _userRepository;
        private readonly IBaseRepository<Event> _eventRepository;
        private readonly IManageTimeRepository _manageTimeRepository;
        public ReserveRepository(IBaseRepository<Reservation> reserveRepository, IBaseRepository<Order> orderRepository, IBaseRepository<AppUser> userRepository, IBaseRepository<Event> eventRepository, IManageTimeRepository manageTimeRepository)
        {
            _reserveRepository = reserveRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _manageTimeRepository = manageTimeRepository;
        }
        #endregion
        public async Task<Reservation> GetReservation(CancellationToken token, string userId)
        {
            var res = await _reserveRepository.GetQuery(i => i.UserId == userId).SingleOrDefaultAsync().ConfigureAwait(false);
            if (res == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return res;
        }
        public async Task<bool> MakeReserve(CancellationToken token, ReservationRequest reservation, int eventId)
        {
            var current = await _eventRepository.GetQuery(i => i.Id == eventId).SingleOrDefaultAsync().ConfigureAwait(false);
            var user = await _userRepository.GetQuery(i => i.Id == reservation.UserId).SingleOrDefaultAsync().ConfigureAwait(false);
            var minutes = await _manageTimeRepository.GetReservationTime(token).ConfigureAwait(false);
            var res = await _reserveRepository.GetQuery(i => i.UserId == user.Id).SingleOrDefaultAsync().ConfigureAwait(false);
            if (res != null)
                throw new AlreadyExistsException(ErrorMessages.AlreadyExists);
            if (current == null || user == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            if (current.Quantity > 0)
            {
                var newReserve = new Reservation()
                {
                    UserId = user.Id,
                    Quantity = reservation.Quantity,
                    Total = reservation.Quantity * current.Price,
                    EventId = eventId,
                    ReservationStatus = false,
                    ReservationTime = DateTime.Now,
                    ReservationEndTime = DateTime.Now.AddMinutes(minutes),
                    Minutes = minutes
                };
                current.Quantity -= reservation.Quantity;
                if (current.Quantity >= 0)
                {
                    await _reserveRepository.Create(token, newReserve).ConfigureAwait(false);
                    _eventRepository.Update(current);
                }
                else
                {
                    throw new IncorrectInfoException(ErrorMessages.IncorrectInfo);
                }
            }
            return await _orderRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task<bool> MakeOrder(CancellationToken token, int reservationId)
        {
            var res = await _reserveRepository.GetQuery(i => i.Id == reservationId).SingleOrDefaultAsync().ConfigureAwait(false);
            if (res == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            var newOrder = new Order()
            {
                UserId = res.UserId,
                Quantity = res.Quantity,
                Total = res.Total,
                EventId = res.EventId,
                Price = res.Total,
                Status = true
            };
            await _orderRepository.Create(token, newOrder).ConfigureAwait(false);
            _reserveRepository.Delete(res);
            return await _orderRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }

        public async Task TerminateOrder(CancellationToken token)
        {
            var res = _reserveRepository.GetQuery();
            var totalCount = await res.CountAsync(token).ConfigureAwait(false);
            var data = await res.ToListAsync(token).ConfigureAwait(false);
            var numberOfChunks = Math.Ceiling(Convert.ToDecimal(totalCount) / 50);

            for (var i = 0; i < numberOfChunks; i++)
            {
                foreach (var ad in data.Skip((i - 1) * 50).Take(50))
                {
                    if (DateTime.Now >= ad.ReservationEndTime)
                    {
                        var ev = await _eventRepository.GetQuery(i => i.Id == ad.EventId).SingleOrDefaultAsync(token).ConfigureAwait(false);
                        if (ev == null)
                            continue;
                        ev.Quantity += ad.Quantity;
                        _eventRepository.Update(ev);
                        _reserveRepository.Delete(ad);
                    }
                }
                await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
            }
        }
    }
}
