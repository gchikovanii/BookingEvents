using ItAcademy.Application.Accounts;
using ItAcademy.Application.Accounts.Dtos;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var users = await _userService.GetUsersWithRoles(token).ConfigureAwait(false);
            return View(users.Adapt<List<UserRoleDto>>());
        }
        [Authorize("Admin")]
        public async Task<IActionResult> Edit(CancellationToken token, string id)
        {
            var user = await _userService.GetUserWithRoles(token, id).ConfigureAwait(false);
            if (user == null)
                return View("NotFound");
            var response = new UserRoleDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = user.Roles
            };
            return View(response);
        }
        [Authorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> EditToModerator(CancellationToken token, string id)
        {
            var user = await _userService.GetUserWithRoles(token, id).ConfigureAwait(false);
            if (user == null)
                return View("NotFound");
            await _userService.MakeModerator(token, id).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(CancellationToken token, string id)
        {
            var user = await _userService.GetUserWithRoles(token, id).ConfigureAwait(false);
            if (user == null)
                return View("NotFound");
            var response = new UserRoleDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                Roles = user.Roles
            };
            return View(response);
        }
        [Authorize("Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(CancellationToken token, string id)
        {
            var user = await _userService.GetUserWithRoles(token, id).ConfigureAwait(false);
            if (user == null)
                return View("NotFound");
            await _userService.DeactivateAccount(token, id).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }
    }
}
