using ItAcademy.API.Infrastructure.Auth.Jwt;
using ItAcademy.Application.Accounts;
using ItAcademy.Application.Accounts.Requests;
using ItAcademy.Domain.UserAggregate;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ItAcademy.API.Controllers
{
    public class AccountController : BaseController
    {
        #region Ctor
        private readonly IUserService _userService;
        private readonly IOptions<JwtConfiguration> _options;
        private readonly UserManager<AppUser> _roles;
        public AccountController(IUserService userService, IOptions<JwtConfiguration> options, UserManager<AppUser> roles)
        {
            _userService = userService;
            _options = options;
            _roles = roles;
        }
        #endregion

        [HttpPost("register")]
        public async Task<string> Register(CancellationToken token, RegisterUserRequest request)
        {
            return await _userService.RegisterUser(token, request).ConfigureAwait(false);
        }
        [HttpPost("access-token")]
        public async Task<string> LogIn(CancellationToken token, LogInUserRequest request)
        {
            var result = await _userService.SignIn(token, request).ConfigureAwait(false);
            var role = await _roles.GetRolesAsync(result.Adapt<AppUser>()).ConfigureAwait(false);
            return JwtHelper.GenerateToken(result.Id, role.FirstOrDefault(), _options);
        }
    }
}
