using ItAcademy.Application.Accounts.Constants;
using ItAcademy.Application.Events.Repositories;
using ItAcademy.Application.Events.Response;
using ItAcademy.Application.Infrastructure.Errors.CustomExceptions;
using ItAcademy.Application.Infrastructure.Localization.Errors;
using ItAcademy.Application.Manage.Repositories;
using ItAcademy.Domain.EventsAggregate;
using ItAcademy.Domain.UserAggregate;
using ItAcademy.Infrastructure.BaseRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItAcademy.Infrastructure.Events
{
    public class EventRepository : IEventRepository
    {
        #region Ctor
        private readonly IBaseRepository<Event> _eventRepository;
        private readonly IBaseRepository<AppUser> _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IManageTimeRepository _manageTimeRepository;

        public EventRepository(IBaseRepository<Event> eventRepository, IBaseRepository<AppUser> userRepository, UserManager<AppUser> userManager, IManageTimeRepository manageTimeRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _manageTimeRepository = manageTimeRepository;
        }
        #endregion
        public async Task<Event> GetEventsForAdminById(CancellationToken token, int eventId)
        {
            return await _eventRepository.GetQuery(i => i.Id == eventId && i.Status == true).Include(i => i.User).SingleOrDefaultAsync().ConfigureAwait(false) ?? throw new DoesntExistsException(ErrorMessages.NotFound);
        }
        public async Task<IEnumerable<EventResponse>> GetEventsForAdmin(CancellationToken token)
        {
            var result = await Task.FromResult(_eventRepository.GetQuery(i => i.Status == true && i.Approved == false).Include(i => i.User)).ConfigureAwait(false);
            return result.Select(i => new EventResponse()
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
                CreatedAt = i.CreatedAt,
                Approved = i.Approved,
                Location = i.Location,
                Price = i.Price,
                Quantity = i.Quantity,
                Email = i.User.Email,
                Image = i.Image
            }).ToList();
        }
        public async Task<IEnumerable<EventResponse>> GetEvents(CancellationToken token)
        {
            var result = await Task.FromResult(_eventRepository.GetQuery(i => i.Status == true && i.Approved == true).Include(i => i.User)).ConfigureAwait(false);
            return result.Select(i => new EventResponse()
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                Status = i.Status,
                StartDate = i.StartDate,
                EndDate = i.EndDate,
                CreatedAt = i.CreatedAt,
                Approved = i.Approved,
                Location = i.Location,
                Price = i.Price,
                Quantity = i.Quantity,
                Email = i.User.Email,
                Image = i.Image
            }).ToList();
        }
        public async Task<Event> GetEventById(CancellationToken token, int eventId)
        {
            return await _eventRepository.GetQuery().SingleOrDefaultAsync(i => i.Id == eventId && i.Status == true && i.Approved == true).ConfigureAwait(false) ?? throw new DoesntExistsException(ErrorMessages.NotFound);
        }
        public async Task<string> AddEvent(CancellationToken token, Event request, string userId)
        {
            var user = await _userRepository.GetQuery(i => i.Id == userId).FirstOrDefaultAsync().ConfigureAwait(false);
            request.Status = true;
            request.Approved = false;
            request.UserId = user.Id;
            await _eventRepository.Create(token, request).ConfigureAwait(false);
            await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
            return user.Id;
        }
        public async Task<bool> ApproveEvent(CancellationToken token, int eventId)
        {
            var ev = await _eventRepository.GetQuery(i => i.Id == eventId).FirstOrDefaultAsync().ConfigureAwait(false);
            ev.Approved = true;
            _eventRepository.Update(ev);
            return await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
        }
        public async Task<Event> UpdateEvent(CancellationToken token, Event request, int eventId, string id)
        {
            var user = await _userManager.FindByIdAsync(id).ConfigureAwait(false);
            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var eventFromDb = await _eventRepository.GetQuery().SingleOrDefaultAsync(i => i.Id == eventId && i.Status == true).ConfigureAwait(false);
            var time = await _manageTimeRepository.GetRestrictionTime(token).ConfigureAwait(false);
            var canEditTill = eventFromDb.CreatedAt.AddDays(time);
            var hasAccess = false;
            foreach (var role in roles)
            {
                if (role == RoleType.Admin.ToString() || role == RoleType.Moderator.ToString())
                {
                    hasAccess = true;
                }
            }
            if (hasAccess && DateTimeOffset.UtcNow <= canEditTill)
            {
                if (eventFromDb != null)
                {
                    if (request.StartDate < eventFromDb.EndDate)
                        eventFromDb.StartDate = request.StartDate;
                    if (request.EndDate > eventFromDb.StartDate)
                        eventFromDb.EndDate = request.EndDate;
                    if (!string.IsNullOrEmpty(request.Description))
                        eventFromDb.Description = request.Description;
                    if (!string.IsNullOrEmpty(request.Title))
                        eventFromDb.Title = request.Title;
                    if (!string.IsNullOrEmpty(request.Location))
                        eventFromDb.Location = request.Location;
                    if (request.Price > 0)
                        eventFromDb.Price = request.Price;
                    if (request.Quantity > 0)
                        eventFromDb.Quantity = request.Quantity;
                    _eventRepository.Update(eventFromDb);
                    await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
                    return eventFromDb;
                }
            }
            if(hasAccess == false)
                throw new DoesntExistsException(ErrorMessages.NotFound);
            throw new DoesNotHaveAccessException(ErrorMessages.NotHaveAccess);
        }
        public async Task<bool> DeleteEvent(CancellationToken token, int eventId)
        {
            var eventToRemove = await _eventRepository.GetQuery().SingleOrDefaultAsync(i => i.Id == eventId && i.Status == true).ConfigureAwait(false);
            if(eventToRemove != null)
            {
                eventToRemove.Status = false;
                eventToRemove.Approved = false;
                return await _eventRepository.SaveChangesAsync(token).ConfigureAwait(false);
            }
            throw new DoesntExistsException(ErrorMessages.NotFound);
        }
    }
}
