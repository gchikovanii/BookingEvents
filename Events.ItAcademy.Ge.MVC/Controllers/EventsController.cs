using System.Security.Claims;
using ItAcademy.Application.Events;
using ItAcademy.Application.Events.Dtos;
using ItAcademy.Application.Events.Request;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> EventForAdmin(CancellationToken token)
        {
            var evs = await _eventService.GetEvenstForAdmin(token).ConfigureAwait(false);
            return View(evs.Adapt<IEnumerable<EventDto>>());
        }
        [AllowAnonymous]
        
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var data = await _eventService.GetEvents(token).ConfigureAwait(false);
            return View(data.Adapt<ICollection<EventDto>>());
        }
        [AllowAnonymous] 
        [HttpGet]
        public async Task<IActionResult> Details(CancellationToken token,int id)
        {
            var ev = await _eventService.GetEvent(token,id).ConfigureAwait(false);
            return View(ev.Adapt<EventDto>());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CancellationToken token, EventDto request)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _eventService.AddEvent(token, request.Adapt<EventRequest>(), userId).ConfigureAwait(false);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Incorrect info";
                return View(request);
            }
        }
        public async Task<IActionResult> Edit(CancellationToken token, int id)
        {
            var details = await _eventService.GetEvent(token, id).ConfigureAwait(false);
            if (details == null)
                return View("NotFound");
            var response = new EventDto()
            {
                Id = details.Id,
                Description = details.Description,
                StartDate = details.StartDate,
                EndDate = details.EndDate,
                Image = details.Image,
                Location = details.Location,
                Price = details.Price,
                Quantity = details.Quantity,
                Title = details.Title
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CancellationToken token, int id, EventDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id != request.Id)
                return View("NotFound");
            await _eventService.UpdateEvent(token, request.Adapt<UpdateEventRequest>(), id, userId).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(CancellationToken token, int id)
        {
            var details = await _eventService.GetEventForAdmin(token, id).ConfigureAwait(false);
            if (details == null)
                return View("NotFound");
            var response = new EventDto()
            {
                Id = details.Id,
                Description = details.Description,
                StartDate = details.StartDate,
                EndDate = details.EndDate,
                Image = details.Image,
                Location = details.Location,
                Price = details.Price,
                Quantity = details.Quantity,
                Title = details.Title
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(CancellationToken token, int id)
        {
            var ev = await _eventService.GetEventForAdmin(token, id).ConfigureAwait(false);
            if (ev == null)
                return View("NotFound");
            await _eventService.RemoveEvent(token, id).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Approve(CancellationToken token, int id)
        {
            var details = await _eventService.GetEventForAdmin(token, id).ConfigureAwait(false);
            if (details == null)
                return View("NotFound");
            var response = new EventDto()
            {
                Id = details.Id,
                Description = details.Description,
                StartDate = details.StartDate,
                EndDate = details.EndDate,
                Image = details.Image,
                Location = details.Location,
                Price = details.Price,
                Quantity = details.Quantity,
                Title = details.Title
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Approved(CancellationToken token, int id)
        {
            var ev = await _eventService.GetEventForAdmin(token, id).ConfigureAwait(false);
            if (ev == null)
                return View("NotFound");
            await _eventService.ApproveEvent(token, id).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }
    }
}
