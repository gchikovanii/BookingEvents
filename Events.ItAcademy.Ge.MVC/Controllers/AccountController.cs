using ItAcademy.Application.Accounts;
using ItAcademy.Application.Accounts.Dtos;
using ItAcademy.Application.Accounts.Requests;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Events.ItAcademy.Ge.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService) => _userService = userService;

        public IActionResult Login()
        {
            return View(new LogInDto());
        }
        [HttpPost]
        public async Task<IActionResult> Login(LogInDto request)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                var result = await _userService.LogIn(request.Adapt<LoginRequest>()).ConfigureAwait(false);
                return RedirectToAction("Index", "Events");
            }
            catch
            {
                TempData["Error"] = "Sorry, Email or password is not correct!";
                return View(request);
            }
        }
        public IActionResult Register()
        {
            return View(new RegisterDto());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto request)
        {
            if (!ModelState.IsValid)
                return View(request);
            try
            {
                var result = await _userService.Register(request.Adapt<RegisterRequest>()).ConfigureAwait(false);
                return View("RegisterCompleted");
            }
            catch
            {
                TempData["Error"] = "User with this credentials Already";
                return View(request);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout().ConfigureAwait(false);
            return RedirectToAction("Index", "Events");
        }

    }
}
