using ItAcademy.Application.Events.Response;
using ItAcademy.Domain.EventsAggregate;

namespace ItAcademy.Application.Events.Repositories
{
    public interface IEventRepository
    {
        Task<Event> GetEventsForAdminById(CancellationToken token, int eventId);
        Task<IEnumerable<EventResponse>> GetEventsForAdmin(CancellationToken token);
        Task<Event> GetEventById(CancellationToken token, int eventId);
        Task<IEnumerable<EventResponse>> GetEvents(CancellationToken token);
        Task<string> AddEvent(CancellationToken token, Event request, string userId);
        Task<bool> ApproveEvent(CancellationToken token, int eventId);
        Task<Event> UpdateEvent(CancellationToken token, Event request, int eventId, string id);
        Task<bool> DeleteEvent(CancellationToken token, int eventId);
    }
}
