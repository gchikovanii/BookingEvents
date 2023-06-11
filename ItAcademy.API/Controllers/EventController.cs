using System.Security.Claims;
using ItAcademy.API.Infrastructure.Examples;
using ItAcademy.Application.Accounts.Constants;
using ItAcademy.Application.Events;
using ItAcademy.Application.Events.Request;
using ItAcademy.Application.Events.Response;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
namespace ItAcademy.API.Controllers
{
    public class EventController : BaseController
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [Produces("application/json")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [HttpGet("{eventId}")]
        public async Task<EventResponse> GetEvent(CancellationToken token, int eventId)
        {
            return await _eventService.GetEvent(token, eventId).ConfigureAwait(false);
        }
        [HttpGet("admin/all")]
        public async Task<IEnumerable<EventResponse>> GetEventsforAdmin(CancellationToken token)
        {
            return await _eventService.GetEvenstForAdmin(token).ConfigureAwait(false);
        }
        [Produces("application/json")]
        [ProducesResponseType(typeof(IEnumerable<EventResponse>),StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK,typeof(EventsExample))]
        [HttpGet("all")]
        public async Task<IEnumerable<EventResponse>> GetEvents(CancellationToken token)
        {
            return await _eventService.GetEvents(token).ConfigureAwait(false);
        }
        [HttpPost("add")]
        public async Task<string> AddEvent(CancellationToken token, EventRequest request,string userId)
        {
            return await _eventService.AddEvent(token, request, userId).ConfigureAwait(false);
        }
        [HttpPut("update/{eventId}")]
        public async Task<EventResponse> UpdateEvent(CancellationToken token, UpdateEventRequest request, int eventId)
        {
            return await _eventService.UpdateEvent(token, request, eventId, UserId()).ConfigureAwait(false);
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
        #region UserName
        private string UserId()
        {
            return HttpContext.User.FindFirstValue("Id");
        }
        #endregion
    }
}
