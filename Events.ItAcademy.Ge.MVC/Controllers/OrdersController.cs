using System.Security.Claims;
using ItAcademy.Application.Events;
using ItAcademy.Application.Orders;
using ItAcademy.Application.Orders.Repositories;
using ItAcademy.Application.Orders.Requests;
using ItAcademy.Domain.OrderAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        #region Ctor
        private readonly IOrderService _orderService;
        private readonly IEventService _eventService;
        private readonly IReserveRepository _reserveRepository;
        public OrdersController(IOrderService orderService, IEventService eventService, IReserveRepository reserveRepository)
        {
            _orderService = orderService;
            _eventService = eventService;
            _reserveRepository = reserveRepository;
        }
        #endregion
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var orders = await _orderService.GetAllOrdersForAdmin(token).ConfigureAwait(false);
            return View(orders);
        }
        public async Task<IActionResult> UserOrder(CancellationToken token)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var orders = await _orderService.GetOrders(token, userId, userRole).ConfigureAwait(false);
            return View(orders);
        }

        public async Task<IActionResult> Order(CancellationToken token, int id)
        {
            var ev = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            if (ev == null)
                return View("NotFound");
            var response = new Order()
            {
                EventId = ev.Id,
                Quantity = ev.Quantity,
                Price = ev.Price
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Order(CancellationToken token, OrderRequest request, int id)
        {
            var ev = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            request.UserId = userId;
            await _orderService.MakeOrder(token, request, ev.Id).ConfigureAwait(false);
            return RedirectToAction(nameof(UserOrder));
        }
    }
}
