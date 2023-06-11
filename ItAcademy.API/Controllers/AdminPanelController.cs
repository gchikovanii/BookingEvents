using System.Security.Claims;
using ItAcademy.API.Infrastructure.Examples;
using ItAcademy.Application.Accounts;
using ItAcademy.Application.Accounts.Constants;
using ItAcademy.Application.Accounts.Responses;
using ItAcademy.Application.Events;
using ItAcademy.Application.Events.Response;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Application.Manage.Requests;
using ItAcademy.Application.Orders;
using ItAcademy.Application.Orders.Responses;
using ItAcademy.Domain.ManageAggregate;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace ItAcademy.API.Controllers
{
    public class AdminPanelController : BaseController
    {
        #region Ctor
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IManageTimeRepository _manageTimeRepository;

        public AdminPanelController(IEventService eventService, IUserService userService, IOrderService orderService, IManageTimeRepository manageTimeRepository)
        {
            _eventService = eventService;
            _userService = userService;
            _orderService = orderService;
            _manageTimeRepository = manageTimeRepository;
        }
        #endregion
        [Authorize]
        [HttpGet("all-users")]
        public async Task<List<UserRepsonse>> GetUserByUserName(CancellationToken token)
        {
            var result = await _userService.GetUsersWithRoles(token).ConfigureAwait(false);
            var role = Role();
            if (role == RoleType.Admin.ToString() || Role() == RoleType.Moderator.ToString())
                return result.Adapt<List<UserRepsonse>>();
            throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
        }

        [Authorize]
        [HttpGet("orders")]
        public async Task<IEnumerable<OrderResponse>> GetAllOrders(CancellationToken token)
        {
            var result = await _orderService.GetAllOrdersForAdmin(token).ConfigureAwait(false);
            var role = Role();
            if (role == RoleType.Admin.ToString() || Role() == RoleType.Moderator.ToString())
                return result.Adapt<IEnumerable<OrderResponse>>();
            throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [HttpGet("events/{eventId}")]
        public async Task<EventResponse> GetEvent(CancellationToken token, int eventId)
        {
            return await _eventService.GetEventForAdmin(token, eventId).ConfigureAwait(false);
        }

        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(EventsExample))]
        [HttpGet("all-events")]
        public async Task<IEnumerable<EventResponse>> GetEvents(CancellationToken token)
        {
            return await _eventService.GetEvenstForAdmin(token).ConfigureAwait(false);
        }

        [Authorize]
        [HttpPost("/restrictTime")]
        public async Task<bool> UpdateRestrictionTime(CancellationToken token, RestrictEventTimeRequest request)
        {
            return await _manageTimeRepository.UpdateRestrictionTime(token, request.Adapt<RestrictEventTime>()).ConfigureAwait(false);
        }
        [Authorize]
        [HttpPost("/reservationTime")]
        public async Task<bool> UpdateReservationTime(CancellationToken token, ReserveTimeRequest request)
        {
            return await _manageTimeRepository.UpdateReservationTime(token, request.Adapt<ReserveTime>()).ConfigureAwait(false);
        }

        [Authorize]
        [HttpPut("approve-events/{eventId}")]
        public async Task<bool> ApproveEvent(CancellationToken token, int eventId)
        {
            if (!IsAdminOrModerator())
                throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
            return await _eventService.ApproveEvent(token, eventId).ConfigureAwait(false);
        }
        [Authorize]
        [HttpPut("make-moderator")]
        public async Task<bool> GiveModerator(CancellationToken token, string userName)
        {
            if (Role() != RoleType.Admin.ToString())
                throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
            return await _userService.MakeModerator(token, userName).ConfigureAwait(false);
        }

        [Authorize]
        [HttpPut("deactivate-account")]
        public async Task<bool> RemoveAccount(CancellationToken token, string userName)
        {
            if (Role() != RoleType.Admin.ToString())
                throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
            return await _userService.DeactivateAccount(token, userName).ConfigureAwait(false);
        }
        [HttpPut("delete-event/{{eventId}}")]
        public async Task<bool> DeleteEvent(CancellationToken token, int eventId)
        {
            if (!IsAdminOrModerator())
                throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
            return await _eventService.RemoveEvent(token, eventId).ConfigureAwait(false);
        }
        #region Roles
        private bool IsAdminOrModerator()
        {
            var role = HttpContext.User.FindFirstValue("Role");
            if (role == RoleType.Moderator.ToString() || role == RoleType.Admin.ToString())
                return true;
            if (role == RoleType.User.ToString())
                return false;
            throw new DoesntExistsException(ErrorMessages.NotFound);
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
