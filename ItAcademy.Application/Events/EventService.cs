using ItAcademy.Application.Events.Repositories;
using ItAcademy.Application.Events.Request;
using ItAcademy.Application.Events.Response;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Domain.EventsAggregate;
using Mapster;

namespace ItAcademy.Application.Events
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<EventResponse> GetEventForAdmin(CancellationToken token, int eventId)
        {
            var response = await _eventRepository.GetEventsForAdminById(token, eventId).ConfigureAwait(false);
            return response.Adapt<EventResponse>();
        }
        public async Task<IEnumerable<EventResponse>> GetEvenstForAdmin(CancellationToken token)
        {
            var events = await _eventRepository.GetEventsForAdmin(token).ConfigureAwait(false);
            return events;
        }
        public async Task<EventResponse> GetEvent(CancellationToken token, int eventId)
        {
            var response = await _eventRepository.GetEventById(token, eventId).ConfigureAwait(false);
            if (response == null)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return response.Adapt<EventResponse>();
        }
        public async Task<IEnumerable<EventResponse>> GetEvents(CancellationToken token)
        {
            var events = await _eventRepository.GetEvents(token).ConfigureAwait(false);
            return events.Adapt<IEnumerable<EventResponse>>();
        }

        public async Task<string> AddEvent(CancellationToken token, EventRequest request, string userId)
        {
            var ev = request.Adapt<Event>();
            if (ev.EndDate < ev.StartDate)
                throw new IncorrectInfoException(ErrorMessages.IncorrectInfo);
            return await _eventRepository.AddEvent(token, ev, userId).ConfigureAwait(false);
        }
        public async Task<bool> ApproveEvent(CancellationToken token, int eventId)
        {
            var result = await _eventRepository.ApproveEvent(token, eventId).ConfigureAwait(false);
            return result;
        }
        public async Task<EventResponse> UpdateEvent(CancellationToken token, UpdateEventRequest request, int eventId, string id)
        {
            var ev = request.Adapt<Event>();
            var result = await _eventRepository.UpdateEvent(token, ev, eventId, id).ConfigureAwait(false);
            if (ev.Id != eventId)
                throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
            if (id == null)
                throw new NullReferenceException(ErrorMessages.IncorrectInfo);
            if (eventId == 0)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return result.Adapt<EventResponse>();
        }
        public async Task<bool> RemoveEvent(CancellationToken token, int eventId)
        {
            var result = await _eventRepository.DeleteEvent(token, eventId).ConfigureAwait(false);
            if (!result)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            return result;
        }
    }
}
