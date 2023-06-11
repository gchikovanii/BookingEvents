using ItAcademy.Application.Events;
using ItAcademy.Application.Orders.Repositories;
using ItAcademy.Application.Orders;
using Microsoft.AspNetCore.Mvc;
using ItAcademy.Application.Orders.Requests;
using System.Security.Claims;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Application.Orders.Reservations.Dtos;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    public class ReservesController : Controller
    {
        #region Ctor
        private readonly IOrderService _orderService;
        private readonly IEventService _eventService;
        private readonly IReserveRepository _reserveRepository;
        private readonly IManageTimeRepository _manageTimeRepository;
        public ReservesController(IOrderService orderService, IEventService eventService, IReserveRepository reserveRepository, IManageTimeRepository manageTimeRepository)
        {
            _orderService = orderService;
            _eventService = eventService;
            _reserveRepository = reserveRepository;
            _manageTimeRepository = manageTimeRepository;
        }
        #endregion

        public async Task<IActionResult> Reserve(CancellationToken token, int id)
        {
            var ev = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            if (ev == null)
                return View("NotFound");
            var response = new ReservationDto()
            {
                EventId = ev.Id,
                Quantity = ev.Quantity,
                Price = ev.Price
            };
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> Reserve(CancellationToken token, ReservationRequest request, int id)
        {
            var ev = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.UserId = userId;
            await _reserveRepository.MakeReserve(token, request, ev.Id).ConfigureAwait(false);
            return RedirectToAction(nameof(ConfirmReservation), new { id = ev.Id});
        }

        public async Task<IActionResult> ConfirmReservation(CancellationToken token, int id)
        {
            var ev = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            if (ev == null)
                return View("NotFound");
            var min = await _manageTimeRepository.GetReservationTime(token).ConfigureAwait(false);
            var response = new ReservationDto()
            {
                EventId = ev.Id,
                Quantity = ev.Quantity,
                Price = ev.Price,
                Minutes = min
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmReservationAction(CancellationToken token)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var reservation = await _reserveRepository.GetReservation(token, userId).ConfigureAwait(false);
            await _reserveRepository.MakeOrder(token,reservation.Id).ConfigureAwait(false);
            return RedirectToAction("Index", "Events");
        }
    }
}
