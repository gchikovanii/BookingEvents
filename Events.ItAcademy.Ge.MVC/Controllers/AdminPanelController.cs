using Microsoft.AspNetCore.Mvc;
using ItAcademy.Application.Manage;
using ItAcademy.Domain.ManageAggregate;
using ItAcademy.Application.Manage.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private readonly IManageTimeService _manageTimeService;
        public AdminPanelController(IManageTimeService manageTimeService) => _manageTimeService = manageTimeService;

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var time = await _manageTimeService.GetReservationTime(token).ConfigureAwait(false);
            if (time == 0)
                return View("NotFound");
            var response = new ReserveTimeDto
            {
                Minutes = time
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Index(CancellationToken token, ReserveTime time)
        {
            await _manageTimeService.UpdateReservationTimeInMinutes(token,time).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> EventRestrict(CancellationToken token)
        {
            var time = await _manageTimeService.GetRestrictionTime(token).ConfigureAwait(false);
            if (time == 0)
                return View("NotFound");
            var response = new RestrictEventTimeDto
            {
                Hours = time
            };
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> EventRestrict(CancellationToken token, RestrictEventTime time)
        {
            await _manageTimeService.UpdateRestrictionTimeInHours(token, time).ConfigureAwait(false);
            return RedirectToAction(nameof(EventRestrict));
        }
    }
}
