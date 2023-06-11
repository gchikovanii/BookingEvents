using System.Security.Claims;
using ItAcademy.Application.Accounts.Constants;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Application.Orders;
using ItAcademy.Application.Orders.Requests;
using ItAcademy.Application.Orders.Responses;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ItAcademy.API.Controllers
{
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize]
        [HttpGet("orders")]
        public async Task<IEnumerable<OrderResponse>> GetOrders(CancellationToken token)
        {
            var userId = UserId();
            var role = Role();
            var orders = await _orderService.GetOrders(token, userId, role).ConfigureAwait(false);
            return orders.Adapt<IEnumerable<OrderResponse>>();
        }
        [Authorize]
        [HttpPost("make-order")]
        public async Task<bool> AddEvent(CancellationToken token, OrderRequest request, int eventId)
        {
            return await _orderService.MakeOrder(token, request, eventId).ConfigureAwait(false);
        }
        #region UserName
        private string UserId()
        {
            return HttpContext.User.FindFirstValue("UserName");
        }
        private string Role()
        {
            var role = HttpContext.User.FindFirstValue("Role");
            if (role == RoleType.Moderator.ToString())
                return RoleType.Moderator.ToString();
            if (role == RoleType.Admin.ToString())
                return RoleType.Admin.ToString();
            if (role == RoleType.User.ToString())
                return RoleType.User.ToString();
            throw new DoesntExistsException(ErrorMessages.NotFound);
        }
        #endregion
    }
}
